using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Errors;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
    public class GenerateaNormalOrderController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly DataContext _dataContext;
        private readonly IDevelopmentHelper _developmentHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly ICreateFileOrFolder _createFileOrFolder;
        private List<TmpOrdersVerificViewModel> ObjSpecialist = new List<TmpOrdersVerificViewModel>();
        private readonly List<TmpOrdersVerificViewModel> ListObjSpecialistIncentiveOrders = new List<TmpOrdersVerificViewModel>();
        List<ObjectAvatarViewModel> OrdersObjecLayout = new List<ObjectAvatarViewModel>();
        // GET: GenerateaNormalOrderController
        public GenerateaNormalOrderController(IUserHelper userHelper,
            IFlashMessage flashMessage, DataContext dataContext,
            IDevelopmentHelper developmentHelper, ICombosHelper combosHelper
            , IOrderRepository orderRepository, IConfiguration configuration, ICreateFileOrFolder createFileOrFolder)
        {
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _dataContext = dataContext;
            _developmentHelper = developmentHelper;
            _combosHelper = combosHelper;
            _orderRepository = orderRepository;
            _configuration = configuration;
            _createFileOrFolder = createFileOrFolder;
        }
        public async Task<ActionResult> Index()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            List<OnlyOrderDetails> ListOrders = await _orderRepository.GetCKOnlyOrdersAsync();

            return View(ListOrders);
        }

        // GET: GenerateaNormalOrderController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            Response<UserManagerEntity> _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            DisUserOrdersVModel disUserOrders = new DisUserOrdersVModel();
            Response<DisUserOrdersVModel> modelorder = await _developmentHelper
                              .GetSqlOnlyOrderRecordsAsync(_users.Result.UserId, id.Value);
            disUserOrders = modelorder.Result;

            GenericResponse<InOrderDetailViewModel> _object = await _developmentHelper.GetSqlAllOrderDetailsRecordsAsync<InOrderDetailViewModel>(id);
            disUserOrders.DetailsOrders = _object.ListResults;
            return View(disUserOrders);
        }

        // GET: GenerateaNormalOrderController/Create
        public async Task<ActionResult> Create()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            Response<UserManagerEntity> _users = await _userHelper.GetUserByEmailAsync(User.Identity.Name);


            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            AddGenerateNormalOrderModel model = new AddGenerateNormalOrderModel
            {
                UserId = _users.Result.UserId,
                KamName = $"{_users.Result.FirstName}{" "}{_users.Result.LastName1}{"  "}{_users.Result.LastName2}",
                OrderDate = DateTime.Now.ToLocalTime(),
                OrderStatusId = Convert.ToInt32("0"),
                CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0"),
                CombosKams = _combosHelper.GetComboKamCoords(),
                CombosTypeofPayments = _combosHelper.GetComboTypeofPayments(),
                GenerateUserId = _users.Result.UserId,
                EmployeeNumber = _users.Result.EmployeeNumber,
                Quantity = 150,
                KamManagerId = _users.Result.KamManagerId,
                GenerateDistributor = 0,
            };
            model.Details = await _developmentHelper.GetSqlDataTmpCKOrders(_users.Result.UserId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddGenerateNormalOrderModel collection)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                try
                {
                    Distributors _dist = await _dataContext
                         .Distributors
                         .FindAsync(collection.DistributorId);

                    if (_dist == null)
                    {
                        return new NotFoundViewResult("_ResourceNotFound");
                    }
                    collection.Debtor = Convert.ToInt32(_dist.Debtor);
                    Response<object> _Result = await _orderRepository.AddItemToGenerateaNormalOrderAsync(collection, User.Identity.Name);
                    if (!_Result.IsSuccess)
                    {
                        _flashMessage.Danger("Incorrect information Order Tmp check", _Result.Message);
                        collection.CombosKams = _combosHelper.GetComboKamCoords();
                        collection.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
                        collection.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
                        collection.Details = await _developmentHelper.GetSqlDataTmpCKOrders(collection.UserId);
                        collection.Quantity = 150;

                        return View(collection);
                    }

                    _flashMessage.Confirmation(string.Empty, "el pedido ya fue registrado se encuentra en status de verificacion.");

                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(string.Empty, ex.Message);
                    collection.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
                    collection.CombosKams = _combosHelper.GetComboKamCoords();
                    collection.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
                    collection.Details = await _developmentHelper.GetSqlDataTmpCKOrders(collection.UserId);
                    return View(collection);
                }
            }
            collection.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
            collection.CombosKams = _combosHelper.GetComboKamCoords();
            collection.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
            collection.Details = await _developmentHelper.GetSqlDataTmpCKOrders(collection.UserId);
            return View(collection);
        }
        // POST: GenerateaNormalOrderController/Create
        [HttpGet]
        public async Task<IActionResult> OrderVerification()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            Response<UserManagerEntity> _UserK = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (_UserK.IsSuccess == false)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            OrderVerificationViewModel model = new OrderVerificationViewModel
            {
                UserId = _UserK.Result.UserId,
                //KamId = _UserK.Result.KamId,
                //CombosDistributors = _combosHelper.GetComboAllKDistributors(),
                CombosKams = _combosHelper.GetComboKamCoords(),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderVerification(OrderVerificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                try
                {
                    Response<object> response = await _orderRepository.AddToOrderGenerateNormalAsync(model, User.Identity.Name);
                    if (!response.IsSuccess)
                    {
                        _flashMessage.Danger(string.Empty, response.Message);
                        model.CombosKams = _combosHelper.GetComboKamCoords();
                        return View(model);
                    }
                    return RedirectToAction(nameof(Index), "GenerateaNormalOrder");
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(string.Empty, exception.Message);
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            model.CombosKams = _combosHelper.GetComboKamCoords();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(long? id) {
            if (id == 0)
            {
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (User.Identity.Name == null)
            {
                var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }

            PrOrders order = await _dataContext
                .PrOrders
                .Where(i => i.OrderId.Equals(id))
                .FirstOrDefaultAsync();

            if (order == null)
            {
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }

            Distributors distributors = await _dataContext
                .Distributors
                .Include(u => u.Users)
                .Where(d => d.DistributorId.Equals(order.DistributorId))
                .FirstOrDefaultAsync();

            if (distributors == null)
            {
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var EmailDetails = await _developmentHelper.GetOnlySqlEmailKamCoordDistrs(order.DistributorId);

            OrdersObjecLayout = await (from pd in _dataContext.PrOrderDetails
                                  join po in _dataContext.PrOrders on pd.OrderId equals po.OrderId
                                  join dw in _dataContext.DeatilWarehouses on pd.DeatilStoreId equals dw.DeatilStoreId
                                  join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                  join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                  join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                  join d in _dataContext.Distributors on po.DistributorId equals d.DistributorId
                                  join pay in _dataContext.TypeofPayments on pd.TypeofPaymentId equals pay.TypeofPaymentId
                                  where po.OrderId == id && pd.OrderStatus == "Sent" && pd.GenerateDistributor == 0
                                  select new ObjectAvatarViewModel
                                  {
                                      Debtor = po.Debtor,
                                      OrderDate = po.OrderDate.ToLocalTime(),
                                      BusinessName = d.BusinessName,
                                      Quantity = pd.Quantity,
                                      OraclepId = p.OraclepId,
                                      Description = p.Description,
                                      ShippingBranchNo = wr.ShippingBranchNo,
                                      ShippingBranchName = wr.ShippingBranchName,
                                      filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}",
                                      OrderCode = $"{pd.OrderCode.Replace("-", "_").Replace(" ", "")}{".xlsx"}",
                                      To = distributors.Users.Email,
                                      OrderId = po.OrderId,
                                      DeatilStoreId = pd.DeatilStoreId,
                                      OrderDetailId = pd.OrderDetailId,
                                      OptionalEmailDetails = EmailDetails,
                                  }).ToListAsync();

            if (OrdersObjecLayout.Count > 0)
            {
                var _response = _createFileOrFolder.WriteExcelFileNormal(OrdersObjecLayout);

                if (!_response.IsSuccess)
                {
                    _flashMessage.Danger($"{"Error Send  :"}{_response.Message}");
                    return RedirectToAction(nameof(Index), new { });
                }
            }
            _flashMessage.Danger($"{"Send  :"}{"Email OK."}");
            return RedirectToAction("Index", "GenerateaNormalOrder");

        }

        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 10);
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -10);
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> DeletedTmp(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            PrOrderDetailTmps tmp = await _dataContext.prOrderDetailTmps.FindAsync(id);
            _dataContext.Remove(tmp);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Create");
        }

        private IReadOnlyList<TmpOrdersVerificViewModel> TblSpecialistDetails(long Id)
        {
            try
            {
                Kams _UseAvatar = _dataContext.Kams
                                   .Include(k => k.Users)
                                   .Where(u => u.Users.UserName == User.Identity.Name)
                                   .FirstOrDefault();

                if (_UseAvatar == null)
                {
                    return ListObjSpecialistIncentiveOrders;
                }
                ObjSpecialist = (from t in _dataContext.prOrderDetailTmps
                                 join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                 join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                 join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                 join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                 join d in _dataContext.Distributors on t.DistributorId equals d.DistributorId
                                 join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                 // where t.GenerateUserId == _UseAvatar.Users.UserId || d.DistributorId == Id
                                 where d.DistributorId == Id
                                 select new TmpOrdersVerificViewModel
                                 {
                                     Debtor = t.Debtor,
                                     BusinessName = d.BusinessName,
                                     Observations = t.Observations,
                                     DeatilStoreId = t.DeatilStoreId,
                                     OrderDetailTmpId = t.OrderDetailTmpId,
                                     Price = t.Price,
                                     Quantity = t.Quantity,
                                     TaxPrice = t.TaxRate,
                                     OrderCode = p.ShortDescription,
                                     OrderStatus = t.OrderStatus,
                                     DeatilProducts = p.Description,
                                     ShippingBranchNo = wr.ShippingBranchNo,
                                     MD = d.MD.ToUpper(),
                                     PayofType = pay.PaymentName,
                                     OraclepId = p.OraclepId,
                                     UserId = t.GenerateUserId.Value,
                                     ShippingBranchName = wr.ShippingBranchName,
                                     ShortDescription = p.ShortDescription,
                                     TypeofPaymentId = t.TypeofPaymentId,
                                 }).ToList();

                int recordsTotal = ObjSpecialist.ToList().Count;
                int count = 1;

                foreach (TmpOrdersVerificViewModel itemIncen in ObjSpecialist)
                {
                    TmpOrdersVerificViewModel tmpo = new TmpOrdersVerificViewModel
                    {
                        Debtor = itemIncen.Debtor,
                        BusinessName = itemIncen.BusinessName,
                        Observations = itemIncen.Observations,
                        DeatilStoreId = itemIncen.DeatilStoreId,
                        OrderDetailTmpId = itemIncen.OrderDetailTmpId,
                        Price = itemIncen.Price,
                        Quantity = itemIncen.Quantity,
                        TaxPrice = itemIncen.TaxPrice,
                        OrderStatus = itemIncen.OrderStatus,
                        DeatilProducts = itemIncen.DeatilProducts,
                        ShippingBranchNo = itemIncen.ShippingBranchNo,
                        ShippingBranchName = itemIncen.ShippingBranchName,
                        OraclepId = itemIncen.OraclepId,
                        ShortDescription = itemIncen.ShortDescription,
                        UserId = itemIncen.UserId,
                        MD = itemIncen.MD,
                        PayofType = itemIncen.PayofType,
                        PaymentMethod = itemIncen.PaymentMethod,
                        UseCfdi = itemIncen.UseCfdi,
                        TypeofPaymentId = itemIncen.TypeofPaymentId,
                        OrderCode = $"{itemIncen.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{" - "}{itemIncen.ShippingBranchNo}{" - "}{itemIncen.OrderCode}{" - "}{itemIncen.PayofType}{" - "}{string.Format("{0:C2}", itemIncen.Quantity * itemIncen.Price)}{" - "}{count}{" de "}{ recordsTotal}",
                    };
                    PrOrderDetailTmps _tmpi = _dataContext
                        .prOrderDetailTmps
                        .Where(i => i.OrderDetailTmpId == tmpo.OrderDetailTmpId)
                        .FirstOrDefault();
                    _tmpi.OrderCode = tmpo.OrderCode;
                    _dataContext.prOrderDetailTmps.Update(_tmpi);
                    _dataContext.SaveChanges();
                    ListObjSpecialistIncentiveOrders.Add(tmpo);
                    count++;
                }
                return ListObjSpecialistIncentiveOrders;
            }
            catch (Exception)
            {
                return ListObjSpecialistIncentiveOrders;
            }
        }

        [HttpPost]
        public JsonResult SpecialistDetails(long distributorId)
        {
            //Creating List

            if (distributorId == 0)
            {
                return new JsonResult(ObjSpecialist.ToList());
            }

            if (User.Identity.Name == null)
            {
                return new JsonResult(ObjSpecialist.ToList());
            }

            try
            {
                long _distributorId = Convert.ToInt64(distributorId);

                IReadOnlyList<TmpOrdersVerificViewModel> ListObjSpecialistOrders = TblSpecialistDetails(_distributorId);
                //return list as Json    
                //return Json(ListObjSpecialistIncentiveOrders.ToList()) ;
                return new JsonResult(ListObjSpecialistOrders.ToList(), new Newtonsoft.Json.JsonSerializerSettings());
            }
            catch (Exception)
            {

                return new JsonResult(ObjSpecialist.ToList(), new Newtonsoft.Json.JsonSerializerSettings());
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
                             where dw.StoreId.Equals(storeid) && dw.IsDeleted == 0
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

        //[HttpGet]
        //public async Task<IActionResult> OrderConfirmation(long? id)
        //{
        //    List<ObjectAvatarViewModel> OrdersLayout = new List<ObjectAvatarViewModel>();
        //    if (id == 0)
        //    {
        //        return new NotFoundViewResult("_ResourceNotFound");
        //    }
        //    if (User.Identity.Name == null)
        //    {
        //        return new NotFoundViewResult("_ResourceNotFound");
        //    }

        //    Response<UserManagerEntity> _UserK = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
        //    if (_UserK.IsSuccess == false)
        //    {
        //        return new NotFoundViewResult("_ResourceNotFound");
        //    }
        //    //var EmailDetails = await _developmentHelper.GetSqlEmailDistrs(_Dusers.Result.Debtor.ToString());
        //    OrdersLayout = await (from t in _dataContext.PrOrderDetails
        //                          join io in _dataContext.PrOrders on t.OrderId equals io.OrderId
        //                          join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
        //                          join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
        //                          join p in _dataContext.Products on dw.ProductId equals p.ProductId
        //                          join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
        //                          join d in _dataContext.Distributors on io.DistributorId equals d.DistributorId
        //                          join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
        //                          where io.OrderId == id && io.OrderStatus == "Sent"
        //                          select new ObjectAvatarViewModel
        //                          {
        //                              Debtor = io.Debtor,
        //                              OrderDate = io.OrderDate.ToLocalTime(),
        //                              BusinessName = d.BusinessName,
        //                              Quantity = t.Quantity,
        //                              OraclepId = p.OraclepId,
        //                              Description = p.Description,
        //                              ShippingBranchNo = wr.ShippingBranchNo,
        //                              ShippingBranchName = wr.ShippingBranchName,
        //                              filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}",
        //                              OrderCode = $"{t.OrderCode.Replace("-", "_").Replace(" ", "")}{".xlsx"}",
        //                              To = _UserK.Result.Email,
        //                              OrderId = io.OrderId,
        //                              DeatilStoreId = t.DeatilStoreId,
        //                              OrderDetailId = t.OrderDetailId,
        //                          }).ToListAsync();

        //    if (OrdersLayout.Count > 0 || OrdersLayout != null)
        //    {
        //        //_createFileOrFolder.WriteExcelFile(OrdersLayout);
        //    }
        //    return RedirectToAction(nameof(Index), "GenerateaNormalOrder");
        //}

        [HttpPost]
        public IActionResult AjaxMethod(string DistributorId)
        {
            if (DistributorId == null)
            {
                return Json(new { result = false, error = "no data" });
            }

            if (User.Identity.Name == null)
            {
                return Json(new { result = false, error = "no data" });
            }

            long _distributorId = Convert.ToInt64(DistributorId);

            IReadOnlyList<TmpOrdersVerificViewModel> ListObjSpecialistOrders = TblSpecialistDetails(_distributorId);
            try
            {

                return Json(new { result = true, ListObjSpecialistOrders });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }
        }
    }
}
