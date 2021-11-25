using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
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
    [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator,KamCoordinator,Kam")]
    public class OrderIncentiveController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IOrderIncentiveRepository _orderIncentiveRepository;
        private readonly IDevelopmentHelper _developmentHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly ICreateFileOrFolder _createFileOrFolder;
        private readonly IConfiguration _configuration;
        private List<TmpIncentiveDViewModel> ObjSpecialist = new List<TmpIncentiveDViewModel>();
        private List<TmpIncentiveDViewModel> ListObjSpecialistIncentiveOrders = new List<TmpIncentiveDViewModel>();
        public OrderIncentiveController(DataContext dataContext
            , IUserHelper userHelper, ICombosHelper combosHelper
            , IOrderIncentiveRepository orderIncentiveRepository
            , IDevelopmentHelper developmentHelper, IFlashMessage flashMessage, ICreateFileOrFolder createFileOrFolder
            , IConfiguration configuration)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _orderIncentiveRepository = orderIncentiveRepository;
            _developmentHelper = developmentHelper;
            _flashMessage = flashMessage;
            _createFileOrFolder = createFileOrFolder;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
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
            var model = await _orderIncentiveRepository
                .GetAllIncentiveOrderRecordsAsync(_users.Result.KamId);
            return View(model);
        }
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = await _orderIncentiveRepository
                .GetOnlyIncentiveOrderRecordsAsync(_users.Result.UserId, id.Value);

            model.DetailsIncentive = await _orderIncentiveRepository.GetAllIncDetailRecordsAsync(id.Value);

            return View(model);
        }
        public async Task<IActionResult> NewOrderIncentive()
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

            var model = new NOrderIncentiveViewModel
            {
                CombosDistributors = _combosHelper.GetComboKDistributors(User.Identity.Name),
                CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0"),
                Quantity = 50,
                Price = 0.01,
            };
            model.CombosTypeofPayments = _combosHelper.GetComboIncTypeofPayments();
            model.DetailsTmp = await _developmentHelper.GetSqlDataTmpOrdersIncentive(_users.Result.UserId);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewOrderIncentive(NOrderIncentiveViewModel model)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _user = await _dataContext.Users
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (_user == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (ModelState.IsValid)
            {
                try
                {

                    Response<object> response = await _orderIncentiveRepository.AddItemToOrderIncentiveAsync(model, User.Identity.Name);
                    if (!response.IsSuccess)
                    {
                        _flashMessage.Danger(response.Message, "Incorrect information check");
                        model.CombosDistributors = _combosHelper.GetComboKDistributors(User.Identity.Name);
                        model.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
                        model.Quantity = 50;
                        model.Price = 0.01;
                        model.CombosTypeofPayments = _combosHelper.GetComboIncTypeofPayments();
                        model.DetailsTmp = await _developmentHelper.GetSqlDataTmpOrdersIncentive(_user.UserId);
                        return View(model);
                    }
                    _flashMessage.Confirmation("Okay", response.Message);
                    return RedirectToAction("NewOrderIncentive", "OrderIncentive", new { });
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message, "Incorrect information check");
                }
            }
            model.CombosDistributors = _combosHelper.GetComboKDistributors(User.Identity.Name);
            model.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
            model.Quantity = 50;
            model.Price = 0.01;
            model.CombosTypeofPayments = _combosHelper.GetComboIncTypeofPayments();
            model.DetailsTmp = await _developmentHelper.GetSqlDataTmpOrdersIncentive(_user.UserId);
            return View(model);
        }
        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                return NotFound();
                //return new StatusCodeResult(StatusCode.BadRequest);
            }
            await _orderIncentiveRepository.ModifyOrderIncentiveDetailTempQuantityAsync(id.Value, 10);
            return RedirectToAction(nameof(NewOrderIncentive));
        }
        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await _orderIncentiveRepository.ModifyOrderIncentiveDetailTempQuantityAsync(id.Value, -10);
            return RedirectToAction(nameof(NewOrderIncentive));
        }
        public async Task<IActionResult> DeletedTmp(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var Incentive = await IncentiveExists(id.Value);

                if (Incentive == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                _dataContext.IncentiveOrderDetailTmp.Remove(Incentive);
                await _dataContext.SaveChangesAsync();
                _flashMessage.Confirmation("The Incentive was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Incentive can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(NewOrderIncentive));
        }
        private async Task<IncentiveOrderDetailTmp> IncentiveExists(int id)
        {
            var _Incentive = await _dataContext.IncentiveOrderDetailTmp.FindAsync(id);
            return _Incentive;
        }
        [HttpGet]
        public async Task<IActionResult> DeliverOrderIncentive(long distributorId)
        {
            //if (id == 0)
            //{
            //    return new NotFoundViewResult("_ResourceNotFound");
            //}

            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _UserK = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (_UserK.IsSuccess == false)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _KAvatar = await _dataContext.Kams
                .Include(u => u.Users)
                .Where(k => k.Users.UserName == _UserK.Result.Username)
                .FirstOrDefaultAsync();

            if (_KAvatar == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = new DeliverOrderIncentiveViewModel
            {
                UserId = _UserK.Result.UserId,
                KamId = _KAvatar.KamId,
                CombosDistributors = _combosHelper.GetComboIdKDistributors(_UserK.Result.MyUserKamId),
            };

            if (distributorId > 0)
            {
                model.DetailsTmp = await _developmentHelper.GetSqlDataTmpSpecialistIncentiveOrders(User.Identity.Name, distributorId);
            }
            else {
                model.DetailsTmp = ListObjSpecialistIncentiveOrders.ToList();
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliverOrderIncentive(DeliverOrderIncentiveViewModel model)
        {
            try
            {

                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                if (model.DistributorId == 0)
                {
                    model.CombosDistributors = _combosHelper.GetComboKDistributors(User.Identity.Name);
                    ModelState.AddModelError(string.Empty, "not Distributor");
                    return View(model);
                }
              var response =  await _orderIncentiveRepository.AddToOrderIncentiveAsync(model, User.Identity.Name);
                if (!response.IsSuccess)
                {
                    _flashMessage.Danger("Error", response.Message);
                    model.CombosDistributors = _combosHelper.GetComboKDistributors(User.Identity.Name);
                    return View(model);
                }
                
                _flashMessage.Confirmation("Okay", response.Message);
                return RedirectToAction("NewOrderIncentive", "OrderIncentive", new { });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                _flashMessage.Confirmation("Exception", exception.Message);
            }
            model.CombosDistributors = _combosHelper.GetComboKDistributors(User.Identity.Name);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> OrderConfirmationI(long? id)
        {
            if (id == 0)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            IncentiveOrders incentive = await _dataContext
                .IncentiveOrders
                .Where(i => i.IncentiveOrderId.Equals(id))
                .FirstOrDefaultAsync();

            if (incentive == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            Distributors distributors = await _dataContext
                .Distributors
                .Include(u => u.Users)
                .Where(d => d.DistributorId.Equals(incentive.DistributorId))
                .FirstOrDefaultAsync();

            //var _UserK = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            
            if (distributors == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var EmailDetails = await _developmentHelper.GetOnlySqlEmailKamCoordDistrs(incentive.DistributorId);

            List<OrdersLayoutModel> OrdersLayout = new List<OrdersLayoutModel>();
            OrdersLayout = await (from t in _dataContext.IncentiveOrderDetails
                                  join io in _dataContext.IncentiveOrders on t.IncentiveOrderId equals io.IncentiveOrderId
                                  join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                  join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                  join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                  join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                  join d in _dataContext.Distributors on io.Debtor equals d.Debtor
                                  join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                  where io.IncentiveOrderId == id && io.OrderStatus == "Sent"
                                  select new OrdersLayoutModel
                                  {
                                      Debtor = io.Debtor,
                                      OrderDate = io.OrderDate.ToLocalTime(),
                                      BusinessName = d.BusinessName,
                                      Quantity = t.Quantity,
                                      OraclepId = p.OraclepId,
                                      Description = p.Description,
                                      Observations = io.Observations,
                                      ShippingBranchNo = wr.ShippingBranchNo,
                                      ShippingBranchName = wr.ShippingBranchName,
                                      filePath = $"{_configuration["FilePath:SecretFilePath"]}",
                                      OrderCode = $"{t.OrderCode.Replace("-", "_").Replace(" ", "")}{".xlsx"}",
                                      To = distributors.Users.Email,
                                      IncentiveOrderId = io.IncentiveOrderId,
                                      DeatilStoreId = t.DeatilStoreId,
                                      IncentiveOrderDetailId = t.IncentiveOrderDetailId,
                                      OptionalEmailDetails = EmailDetails,
                                  }).ToListAsync();

            if (OrdersLayout.Count > 0)
            {
                var _response = _createFileOrFolder.WriteExcelFile(OrdersLayout);

                if (!_response.IsSuccess)
                {
                    _flashMessage.Danger($"{"Error Send  :"}{_response.Message}");
                    return RedirectToAction(nameof(Index), new {});
                }
            }
            _flashMessage.Danger($"{"Send  :"}{"Email OK."}");
            return RedirectToAction("Index", "OrderIncentive");
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
        [HttpGet]
        public JsonResult GetSubProduct(long StoreId)
        {
            List<DwhModel> subStoreslist = new List<DwhModel>();

            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subStoreslist = (from subStore in _dataContext.DeatilWarehouses
                             join p in _dataContext.Products on subStore.ProductId equals p.ProductId
                             where subStore.StoreId == StoreId
                             select new DwhModel
                             {
                                 DeatilStoreId = subStore.DeatilStoreId,
                                 Description = p.Description
                             }).ToList();

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new DwhModel { DeatilStoreId = 0, Description = "*Seleccione un Producto*" });
            return Json(new SelectList(subStoreslist, "DeatilStoreId", "Description"));
        }

        [HttpGet]
        public JsonResult GetSubStores(long DistributorId)
        {
            List<Warehouses> subStoreslist = new List<Warehouses>();

            // ------- Getting Data from Database Using EntityFrameworkCore -------
            subStoreslist = (from subStore in _dataContext.Warehouses
                             where subStore.DistributorId == DistributorId
                             select new Warehouses
                             {
                                 StoreId = subStore.StoreId,
                                 ShippingBranchName = subStore.ShippingBranchName,
                             }).ToList();

            if (subStoreslist.Count == 0)
            {
                subStoreslist.Insert(0, new Warehouses { StoreId = 0, ShippingBranchName = "Select a Warehouses" });
                return Json(new SelectList(subStoreslist, "StoreId", "ShippingBranchName"));
            }

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new Warehouses { StoreId = 0, ShippingBranchName = "*Selecciona un almacen*" });
            return Json(new SelectList(subStoreslist, "StoreId", "ShippingBranchName"));
        }

        [HttpPost]
        public JsonResult OnChangeAutoWarehouses(string DistributorId)
        {
            long distributorid = Convert.ToInt64(DistributorId);
            var DWare = (from st in _dataContext.Warehouses
                         where st.DistributorId.Equals(distributorid)
                         select new
                         {
                             StoreId = st.StoreId,
                             ShippingBranchName = st.ShippingBranchName,
                         }).ToList();

            return new JsonResult(DWare);
        }

        [HttpPost]
        public JsonResult OnChangeAutoPriceComplete(string DeatilStoreId)
        {
            long deatilStoreId = Convert.ToInt64(DeatilStoreId);
            var Priceare = (from dw in _dataContext.DeatilWarehouses
                         join p in _dataContext.Products on dw.ProductId equals p.ProductId
                         where dw.DeatilStoreId.Equals(deatilStoreId)
                         select new PriceViewModel
                         {
                             Price = p.Price.ToString(),
                         }).FirstOrDefault();

            return Json(Priceare); ;
        }
        [HttpPost]
        public JsonResult SpecialistDetails(long distributorId)
        {
            
            //Creating List
            if (User.Identity.Name == null)
            {
                return Json(null);
            }
            var _UseAvatar = _dataContext.Kams
                    .Include(k => k.Users)
                    .Where(u => u.Users.UserName == User.Identity.Name)
                    .FirstOrDefault();

            if (_UseAvatar == null)
            {
                return Json(null);
            }
            ObjSpecialist = (from t in _dataContext.IncentiveOrderDetailTmp
                            join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                            join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                            join p in _dataContext.Products on dw.ProductId equals p.ProductId
                            join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                            join d in _dataContext.Distributors on t.Debtor equals d.Debtor
                            join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                            where t.DistributorId == distributorId
                            select new TmpIncentiveDViewModel
                            {
                                Debtor = t.Debtor,
                                BusinessName = d.BusinessName,
                                Observations = t.Observations,
                                DeatilStoreId = t.DeatilStoreId,
                                IncentiveId = t.IncentiveId,
                                Price = t.Price,
                                Quantity = t.Quantity,
                                TaxPrice = t.TaxPrice,
                                OrderCode = p.ShortDescription,
                                OrderStatus = t.OrderStatus,
                                DeatilProducts = p.Description,
                                ShippingBranchNo = wr.ShippingBranchNo,
                                MD = d.MD.ToUpper(),
                                PayofType = pay.PaymentName,
                                OraclepId = p.OraclepId,
                                ShippingBranchName = wr.ShippingBranchName,
                                ShortDescription = p.ShortDescription,

                            }).ToList();

            int recordsTotal = ObjSpecialist.Count;
            int count = 1;
            
            foreach (var itemIncen in ObjSpecialist)
            {
                var tmpo = new TmpIncentiveDViewModel
                {
                    Debtor = itemIncen.Debtor,
                    BusinessName = itemIncen.BusinessName,
                    Observations = itemIncen.Observations,
                    DeatilStoreId = itemIncen.DeatilStoreId,
                    IncentiveId = itemIncen.IncentiveId,
                    Price = itemIncen.Price,
                    Quantity = itemIncen.Quantity,
                    TaxPrice = itemIncen.TaxPrice,
                    OrderStatus = itemIncen.OrderStatus,
                    DeatilProducts = itemIncen.DeatilProducts,
                    ShippingBranchNo = itemIncen.ShippingBranchNo,
                    ShippingBranchName = itemIncen.ShippingBranchName,
                    OraclepId = itemIncen.OraclepId,
                    MD = itemIncen.MD,
                    PayofType= itemIncen.PayofType,
                    TypeofPaymentId = itemIncen.TypeofPaymentId,
                    ShortDescription = itemIncen.ShortDescription,
                    OrderCode = $"{itemIncen.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{" - "}{itemIncen.ShippingBranchNo}{" - "}{itemIncen.OrderCode}{" - "}{itemIncen.PayofType}{" - "}{string.Format("{0:C2}", itemIncen.Quantity * itemIncen.Price)}{" - "}{count}{" de "}{ recordsTotal}",
                };
                var _tmpi = _dataContext
                    .IncentiveOrderDetailTmp
                    .Where(i => i.IncentiveId == tmpo.IncentiveId)
                    .FirstOrDefault();
                _tmpi.OrderCode = tmpo.OrderCode;
                _dataContext.IncentiveOrderDetailTmp.Update(_tmpi);
                _dataContext.SaveChanges();
                ListObjSpecialistIncentiveOrders.Add(tmpo);
                count++;
            }

            if (ListObjSpecialistIncentiveOrders.Count == 0)
            {
                return Json(null);
            }
            //return list as Json    
            return Json(ListObjSpecialistIncentiveOrders.ToList(), new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
