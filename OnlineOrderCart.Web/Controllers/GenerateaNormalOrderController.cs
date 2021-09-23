using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
    public class GenerateaNormalOrderController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly DataContext _dataContext;
        private readonly IDevelopmentHelper _developmentHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IOrderRepository _orderRepository;

        // GET: GenerateaNormalOrderController
        public GenerateaNormalOrderController(IUserHelper userHelper,
            IFlashMessage flashMessage, DataContext dataContext,
            IDevelopmentHelper developmentHelper, ICombosHelper combosHelper, IOrderRepository orderRepository)
        {
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _dataContext = dataContext;
            _developmentHelper = developmentHelper;
            _combosHelper = combosHelper;
            _orderRepository = orderRepository;
        }
        public async Task<ActionResult> Index()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var ListOrders = await _orderRepository.GetCKOnlyOrdersAsync();

            return View(ListOrders);
        }

        // GET: GenerateaNormalOrderController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View();
        }

        // GET: GenerateaNormalOrderController/Create
        public async Task<ActionResult> Create()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);


            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new AddGenerateNormalOrderModel
            {
                UserId = _users.Result.UserId,
                KamName = $"{_users.Result.FirstName}{" "}{_users.Result.LastName1}{"  "}{_users.Result.LastName2}",
                OrderDate = DateTime.Now.ToLocalTime(),
                OrderStatusId = Convert.ToInt32("0"),
                CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0"),
            };
            model.Details = await _developmentHelper.GetSqlDataTmpCKOrders();

            return View(model);
        }
    

        // POST: GenerateaNormalOrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> AddProduct()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var model = new GenerateaNormalOrderViewModel
            {
                CombosKams = _combosHelper.GetComboKamCoords(),
                CombosTypeofPayments = _combosHelper.GetComboTypeofPayments(),
                UserId = _users.Result.UserId,
                EmployeeNumber =_users.Result.EmployeeNumber, 
                Quantity = 150,
            };
            
            return View(model);
        }
        // GET: GenerateaNormalOrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GenerateaNormalOrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GenerateaNormalOrderController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: GenerateaNormalOrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult OnDistChangeAutoComplete(string KamId)
        {
            long kamid = Convert.ToInt64(KamId);
            var Dist = (from d in _dataContext.Distributors
                         join k in _dataContext.Kams on d.KamId equals k.KamId
                         where d.KamId.Equals(kamid)
                         select new
                         {
                             DistributorId = d.DistributorId,
                             Description = d.BusinessName,
                         }).ToList();

            return new JsonResult(Dist);
        }

        [HttpGet]
        public JsonResult GetSubOnDist(long KamId)
        {
            List<Distributors> subStoreslist = new List<Distributors>();
            long kamid = Convert.ToInt64(KamId);
            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subStoreslist = (from d in _dataContext.Distributors
                             join k in _dataContext.Kams on d.KamId equals k.KamId
                             where d.KamId.Equals(kamid) && d.IsDeleted == 0
                             select new Distributors
                             {
                                 DistributorId = d.DistributorId,
                                 BusinessName = d.BusinessName
                             }).ToList();

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new Distributors { DistributorId = 0, BusinessName = "selecciona un Distribuidor" });


            return Json(new SelectList(subStoreslist, "DistributorId", "BusinessName"));
        }

        [HttpGet]
        public JsonResult GetSubOnWarehouses(long DistributorId)
        {
            List<Warehouses> subStoreslist = new List<Warehouses>();
            long distid = Convert.ToInt64(DistributorId);
            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subStoreslist = (from w in _dataContext.Warehouses
                             join d in _dataContext.Distributors on w.DistributorId equals d.DistributorId
                             where d.DistributorId.Equals(distid) && w.IsDeleted == 0
                             select new Warehouses
                             {
                                 StoreId = w.StoreId,
                                 ShippingBranchName = w.ShippingBranchName
                             }).ToList();

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new Warehouses { StoreId = 0, ShippingBranchName = "selecciona un Almacen" });


            return Json(new SelectList(subStoreslist, "StoreId", "ShippingBranchName"));
        }

        [HttpGet]
        public JsonResult GetSubOnDeatilStores(long StoreId)
        {
            List<DeatilWarehousesViewModel> subStoreslist = new List<DeatilWarehousesViewModel>();
            long storeid = Convert.ToInt64(StoreId);
            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subStoreslist = (from dw in _dataContext.DeatilWarehouses
                             join w in _dataContext.Warehouses on dw.StoreId equals w.StoreId
                             where dw.StoreId.Equals(storeid) && w.IsDeleted == 0
                             select new DeatilWarehousesViewModel
                             {
                                 DeatilStoreId = dw.DeatilStoreId,
                                 Description = dw.Products.Description
                             }).ToList();

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new DeatilWarehousesViewModel { DeatilStoreId = 0, Description = "selecciona un Producto" });


            return Json(new SelectList(subStoreslist, "DeatilStoreId", "Description"));
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

            return new JsonResult(DWare);
        }
    }
}
