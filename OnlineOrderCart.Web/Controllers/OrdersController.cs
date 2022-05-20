using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Errors;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using OnlineOrderCart.Web.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,Distributor")]
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
        private readonly IConfiguration _configuration;
        private readonly ICreateFileOrFolder _createFileOrFolder;
        private readonly IDapperHelper _dapperHelper;

        public OrdersController(DataContext dataContext
            , IUserHelper userHelper, IDistributorHelper distributorHelper
            , ICombosHelper combosHelper, IOrderRepository orderRepository, IFlashMessage flashMessage,
            IConverterHelper converterHelper, IDevelopmentHelper developmentHelper, IConfiguration configuration
            , ICreateFileOrFolder createFileOrFolder, IDapperHelper dapperHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _distributorHelper = distributorHelper;
            _combosHelper = combosHelper;
            _orderRepository = orderRepository;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
            _developmentHelper = developmentHelper;
            _configuration = configuration;
            _createFileOrFolder = createFileOrFolder;
            _dapperHelper = dapperHelper;
        }
        public async Task<IActionResult> IndexOrders()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _Dist = await _dataContext.Distributors
                .Include(u => u.Users)
                .Where(d => d.Users.UserName == User.Identity.Name).FirstOrDefaultAsync();

            var ListOrders = await _orderRepository.GetOnlyOrdersAsync(_Dist.DistributorId);

            return View(ListOrders);
        }
        public async Task<IActionResult> NewOrder()
        {

            if (User.Identity.Name == null){
               var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty,$"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
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
                GenerateDistributor = 1,
                //Details = _dataContext.prOrderDetailTmps.Where(odt => odt.Debtor == _users.Result.Debtor.ToString()).ToList(),
                CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0"),
            };
            model.Details = await _developmentHelper.GetSqlDataTmpOrders(_users.Result.DistributorId);

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> NewOrder(NewOrderModel model)
        {

            if (ModelState.IsValid)
            {

                var _Result = await _orderRepository.ConfirmOrderAsync(model);
                if (!_Result.IsSuccess)
                {
                    model.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
                    model.Details = await _developmentHelper.GetSqlDataTmpOrders(model.DistributorId);
                    _flashMessage.Danger(string.Empty, "Incorrect information check");
                    return View(model);
                }
                _flashMessage.Confirmation(string.Empty, "Correct information");
                return this.RedirectToAction("IndexOrders", "Orders");
            }
            model.CombosOrderStatuses = _combosHelper.GetOrderStatuses().Where(os => os.Value == "0");
            model.Details = await _developmentHelper.GetSqlDataTmpOrders(model.DistributorId);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddItemViewModel model) {
            if (ModelState.IsValid) {
                try {
                    var _Result = await _orderRepository.AddItemToOrderAsync(model, User.Identity.Name);
                    if (!_Result.IsSuccess)
                    {
                        _flashMessage.Danger("Incorrect information Order Tmp check", _Result.Message);
                        model.CombosWarehouses = _combosHelper.GetComboWarehouses(model.DistributorId);
                        model.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
                        model.Quantity = 150;
                        return View(model);
                    }
                    _flashMessage.Confirmation(string.Empty,"Correct information Order Tmp check");
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
                var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = new AddItemViewModel
            {
                CombosWarehouses = _combosHelper.GetComboWarehouses(_users.Result.DistributorId),
                CombosTypeofPayments = _combosHelper.GetComboTypeofPayments(),
                DistributorId = _users.Result.DistributorId,
                Debtor = _users.Result.Debtor,
                UserId = _users.Result.UserId,
                KamId = _users.Result.KamId,
                EmployeeNumber = _users.Result.EmployeeNumber,
                GenerateDistributor = 1,
                Quantity = 150,
            };
            //return PartialView(model);
            return View(model);
        }
        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
                //return new StatusCodeResult(StatusCode.BadRequest);
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 10);
            return this.RedirectToAction("NewOrder");
        }
        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
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
        public async Task<IActionResult> OrderDetails(long? id)
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            DisUserOrdersVModel disUserOrders = new DisUserOrdersVModel();

            var modelorder = await _developmentHelper.GetSqlOnlyOrderRecordsAsync(_users.Result.UserId, id.Value);
            disUserOrders = modelorder.Result;
            GenericResponse<InOrderDetailViewModel> _object = await _developmentHelper.GetSqlAllOrderDetailsRecordsAsync<InOrderDetailViewModel>(id);
            disUserOrders.DetailsOrders = _object.ListResults;
            return View(disUserOrders);
        }
        public async Task<IActionResult> Histories()
        {
            if (User.Identity.Name == null)
            {
                var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = new OrderDetailsViewModel();

            model.OrdersDetails = await _developmentHelper.GetSqlDataOrderDetailsVMl(_users.Result.DistributorId);
            model.IncentiveOrdersDetails = await _developmentHelper.GetSqlDataIncentiveOrdersVMl(_users.Result.DistributorId);
            return View(model);
        }
        public async Task<IActionResult> OrderConfirm(long? id)
        {
            if (User.Identity.Name == null)
            {
                var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (id == null)
            {
                var errorResponse = new CodeErrorResponse(404);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _Dusers = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_Dusers.IsSuccess || _Dusers.Result == null)
            {
                var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var EmailDetails = await _developmentHelper.GetOnlySqlEmailDistrs(_Dusers.Result.DistributorId);
            List<ObjectAvatarViewModel> OrdersLayout = new List<ObjectAvatarViewModel>();
            OrdersLayout = await (from t in _dataContext.PrOrderDetails
                                  join io in _dataContext.PrOrders on t.OrderId equals io.OrderId
                                  join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                  join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                  join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                  join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                  join d in _dataContext.Distributors on io.Debtor equals d.Debtor
                                  join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                  where io.OrderId == id && io.OrderStatus == "Sent"
                                  select new ObjectAvatarViewModel
                                  {
                                      Debtor = io.Debtor,
                                      OrderDate = io.OrderDate.ToLocalTime(),
                                      BusinessName = d.BusinessName,
                                      Quantity = t.Quantity,
                                      OraclepId = p.OraclepId,
                                      Description = p.Description,
                                      ShippingBranchNo = wr.ShippingBranchNo,
                                      ShippingBranchName = wr.ShippingBranchName,
                                      filePath = $"{_configuration["FilePath:SecretNormalFilePath"]}",
                                      OrderCode = $"{t.OrderCode.Replace("-", "_").Replace(" ", "")}{".xlsx"}",
                                      To = _Dusers.Result.Email,
                                      OrderId = io.OrderId,
                                      DeatilStoreId = t.DeatilStoreId,
                                      OrderDetailId = t.OrderDetailId,
                                      OptionalEmailDetails = EmailDetails,
                                  }).ToListAsync();

            if (OrdersLayout.Count > 0)
            {
                var _response = _createFileOrFolder.WriteExcelGenerateReport(OrdersLayout);

                if (!_response.IsSuccess)
                {

                }
            }
            return RedirectToAction("IndexOrders", "Orders");

        }
        [HttpPost]
        public JsonResult OnChangeAutoComplete(string StoreId)
        {
            long storeid = Convert.ToInt64(StoreId);
            var DWare = (from st in _dataContext.DeatilWarehouses
                              where st.StoreId.Equals(storeid) && st.IsDeleted == 0
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
                               where subStore.StoreId == StoreId && p.IsDeleted == 0
                               select new DwhModel
                               { DeatilStoreId = subStore.DeatilStoreId, Description = p.Description 
                               }).ToList();

            // ------- Inserting Select Item in List -------
            subStoreslist.Insert(0, new DwhModel { DeatilStoreId = 0, Description = "Select a products" });


            return Json(new SelectList(subStoreslist, "DeatilStoreId", "Description"));
        }
        public async Task<IActionResult> LoadaddProductPopup()
        {
            AddItemViewModel _model = new AddItemViewModel();
            try
            {
                var _users = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                var errorResponse = new CodeErrorResponse(401);
                _flashMessage.Danger(string.Empty, $"{errorResponse.StatusCode}{" "}{errorResponse.Message}");
                return new NotFoundViewResult("_ResourceNotFound");
            }

                _model.CombosWarehouses = _combosHelper.GetComboWarehouses(_users.Result.DistributorId);
                _model.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
                _model.DistributorId = _users.Result.DistributorId;
                _model.Debtor = _users.Result.Debtor;
                _model.UserId = _users.Result.UserId;
                _model.KamId = _users.Result.KamId;
                _model.EmployeeNumber = _users.Result.EmployeeNumber;
                _model.GenerateDistributor = 1;
                _model.Quantity = 150;
            
                return PartialView("_AddProduct",_model);
            }
            catch (Exception)
            {
                return PartialView("_AddProduct", _model);
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddPartialProduct([FromBody] JObject dataObj)
        {
            string status = "success";
            try
            {
                AddItemViewModel model = new AddItemViewModel {
                    Debtor = dataObj["Debtor"].Value<int>(),
                    DistributorId = dataObj["DistributorId"].Value<int>(),
                    DeatilStoreId = dataObj["DeatilStoreId"].Value<int>(),
                    EmployeeNumber = dataObj["EmployeeNumber"].Value<string>(),
                    KamId = dataObj["KamId"].Value<int>(),
                    Quantity = dataObj["Quantity"].Value<int>(),
                    StoreId = dataObj["StoreId"].Value<int>(),
                    TypeofPaymentId = dataObj["TypeofPaymentId"].Value<int>(),
                    UserId = dataObj["UserId"].Value<int>(),
                    Observations = dataObj["Observations"].Value<string>(),
                    GenerateDistributor = dataObj["GenerateDistributor"].Value<int>()
                };
                var _Result = await _orderRepository.AddItemToOrderAsync(model, User.Identity.Name);
                if (!_Result.IsSuccess)
                {
                    status = "Error";
                    _flashMessage.Danger("Incorrect information Order Tmp check", _Result.Message);
                    model.CombosWarehouses = _combosHelper.GetComboWarehouses(model.DistributorId);
                    model.CombosTypeofPayments = _combosHelper.GetComboTypeofPayments();
                    model.Quantity = 150;
                    return Json(status);
                }
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            return Json(status);

        }

        public async Task<IActionResult> Details(long? id)
        {
           
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var _users = await _distributorHelper.GetDistrByEmailAsync(User.Identity.Name);
            if (!_users.IsSuccess || _users.Result == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            string Query = $"{"Select o.OrderId as 'OrdenId', o.OrderDate, o.OrderStatus, o.DeliveryDate as 'FechaEntrega',dbo.[fun_OnlyDistKams](o.UserId) as 'Operador', d.BusinessName , d.Debtor as 'Deudor' , d.DistributorId, u.UserId  ,o.IsDeleted "}{" from dbo.PrOrders o with(Nolock) Inner Join dbo.Users u With(Nolock) on o.UserId= u.UserId "}{" Inner Join dbo.Distributors d With(Nolock) on o.DistributorId = d.DistributorId Where o.OrderId ="}{id}";
            var dbparams = new DynamicParameters();
            dbparams.Add("Id", id, DbType.Int32);
            var result = await Task.FromResult(_dapperHelper.GetOnlyAvatar<OrderUsDistDto>(Query
                , null,
                commandType: CommandType.Text));

            if (!result.IsSuccess)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            string ListQuery = $"{"Select w.ShippingBranchName, w.ShippingBranchNo , od.Quantity, od.Price, od.TaxRate, od.OrderStatus, od.OrderCode,p.Description, tp.PaymentName, p.ShortDescription "}{" from dbo.PrOrderDetails od With(Nolock) Inner Join  dbo.DeatilWarehouses dw With(Nolock) on od.DeatilStoreId = dw.DeatilStoreId "}{" Inner Join dbo.Warehouses w With(Nolock) on dw.StoreId = w.StoreId Inner Join dbo.Products p With(Nolock) on dw.ProductId= p.ProductId Inner Join dbo.TypeofPayments tp With(Nolock) on od.TypeofPaymentId = tp.TypeofPaymentId Where od.OrderId = "}{id}";
            
            var ListOrderDertail = await _dapperHelper.GetAllAsync<OrderDetailDistDto>(ListQuery, null,
                commandType: CommandType.Text);

            if (!ListOrderDertail.IsSuccess)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            OrderUsDistDto orderUsDistDto = new OrderUsDistDto {
                BusinessName = result.Result.BusinessName,
                Deudor = result.Result.Deudor,
                DistributorId = result.Result.DistributorId,
                Operador = result.Result.Operador,
                OrdenId = result.Result.OrdenId,
                UserId = result.Result.UserId,
                OrderDate = result.Result.OrderDate,
                OrderStatus = result.Result.OrderStatus,
                FechaEntrega = result.Result.FechaEntrega,
                IsDeleted = result.Result.IsDeleted,
                RegistrationDate = result.Result.RegistrationDate,
                OrderDetailDist = ListOrderDertail.ListResults.ToList(),
            };
          return View(orderUsDistDto);
        }
    }
}
