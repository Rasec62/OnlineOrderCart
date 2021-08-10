using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IDistributorHelper _distributorHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;
        private readonly IDevelopmentHelper _developmentHelper;

        public OrdersController(DataContext dataContext
            , IUserHelper userHelper, IDistributorHelper distributorHelper
            , ICombosHelper combosHelper, IOrderRepository orderRepository, IFlashMessage flashMessage,
            IConverterHelper converterHelper, IDevelopmentHelper developmentHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _distributorHelper = distributorHelper;
            _combosHelper = combosHelper;
            _orderRepository = orderRepository;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
            _developmentHelper = developmentHelper;
        }

        public async Task<IActionResult> IndexOrders()
        {
            var ListOrders = await _dataContext
                .PrOrders
                .Include(o => o.Distributors)
                .Include(o => o.GetOrderDetails)
                .ThenInclude(od => od.DeatilWarehouses.GetPrOrderDetails)
                .Where(o => o.IsDeleted == 0)
                .ToListAsync();

            return View(ListOrders);
        }

        public async Task<IActionResult> NewOrder()
        {

            if (User.Identity.Name == null){
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null){
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new NewOrderModel {
                Debtor = _users.Result.Debtor.ToString(),
                BusinessName = _users.Result.BusinessName,
                DistributorId = _users.Result.DistributorId,
                UserId = _users.Result.UserId,
                KamName = _users.Result.KamName,
                OrderCode = $"{_users.Result.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{"-"}",
                OrderDate = DateTime.Now.ToLocalTime(),
                RegistrationDate = DateTime.Now.ToLocalTime(),
                //Details = _dataContext.prOrderDetailTmps.Where(odt => odt.Debtor == _users.Result.Debtor.ToString()).ToList(),
                CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0"),
            };
            model.Details = await _developmentHelper.GetSqlDataTmpOrders(_users.Result.Debtor.ToString());

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddItemViewModel model) {
            if (ModelState.IsValid) {
                try {
                    var _model = await _converterHelper.ToOrdersTmpEntity(model, true);
                    _dataContext.prOrderDetailTmps.Add(_model);
                    await _dataContext.SaveChangesAsync();

                    return this.RedirectToAction("NewOrder");
                }
                catch (SqlException sqlEx) {
                    _flashMessage.Danger(sqlEx.Message, "Incorrect information Sql check");
                }
                catch (Exception ex) {

                    _flashMessage.Danger(ex.Message, "Incorrect information check");
                }
            }
            model.CombosWarehouses = _combosHelper.GetComboWarehouses(model.DistributorId);
            model.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
            model.Quantity = 150;
            return View(model);
        }
        public async Task<IActionResult> AddProduct()
        {

            var _users = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = new AddItemViewModel
            {
                CombosWarehouses = _combosHelper.GetComboWarehouses(_users.Result.DistributorId),
                CombosTypeofPayments = _combosHelper.GetComboTypeofPayments(),
                DistributorId = _users.Result.DistributorId,
                Debtor = _users.Result.Debtor,
                //CombosDWProducts = _combosHelper.GetNextDWComboProducts(_Storeid),
                Quantity = 150,
            };
            //return PartialView(model);
            return View(model);
        }
        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                return NotFound();
                //return new StatusCodeResult(StatusCode.BadRequest);
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 10);
            return this.RedirectToAction("NewOrder");
        }

        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -10);
            return this.RedirectToAction("NewOrder");
        }

        public async Task<IActionResult> DeletedTmp(int? id) {
            if (id == null)
            {
                return NotFound();
            }
            var tmp = await _dataContext.prOrderDetailTmps.FindAsync(id);
            _dataContext.Remove(tmp);
            await _dataContext.SaveChangesAsync();
            return this.RedirectToAction("NewOrder");
        }

        [HttpPost]
        public JsonResult OnChangeAutoComplete(string StoreId)
        {
            long storeid = Convert.ToInt64(StoreId);
            var DWare = (from st in _dataContext.DeatilWarehouses
                              where st.StoreId.Equals(storeid)
                              select new
                              {
                                  DeatilStoreId = st.DeatilStoreId,
                                  Description = st.Products.Description,
                              }).ToList();

            return new JsonResult(new SelectList(DWare, "DeatilStoreId", "Description"));
        }
        [HttpGet]
        public JsonResult GetSubProduct(long StoreId)
        {
            List<DwhModel> subStoreslist = new List<DwhModel>();

            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subStoreslist = (from subStore in _dataContext.DeatilWarehouses
                             join p in _dataContext.Products on subStore.ProductId equals p.ProductId
                               where subStore.StoreId == StoreId
                               select new DwhModel
                               { DeatilStoreId = subStore.DeatilStoreId, Description = p.Description 
                               }).ToList();

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new DwhModel { DeatilStoreId = 0, Description = "Select a products" });


            return Json(new SelectList(subStoreslist, "DeatilStoreId", "Description"));
        }
    }
}
