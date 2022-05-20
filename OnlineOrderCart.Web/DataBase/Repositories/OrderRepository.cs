using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class OrderRepository : GenericRepository<PrOrders>, IOrderRepository
    {
        private readonly DataContext _dataContext;
        private readonly IDistributorHelper _distributorHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;

        public OrderRepository(DataContext datacontext,
            IDistributorHelper distributorHelper,IUserHelper userHelper, ICombosHelper combosHelper, IConverterHelper converterHelper) : base(datacontext)
        {
            _dataContext = datacontext;
            _distributorHelper = distributorHelper;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
        }

        public async Task<Response<object>> AddItemToOrderAsync(AddGenerateNormalOrderModel model, string userName)
        {
            try
            {
                var _combo = _combosHelper
                    .GetOrderStatuses()
                    .Where(s => s.Value == "0")
                    .FirstOrDefault();

                var tmp = _dataContext.prOrderDetailTmps
                   .Where(odt => odt.Debtor == model.Debtor.ToString()
                   && odt.DeatilStoreId == model.DeatilStoreId && odt.TypeofPaymentId.Equals(model.TypeofPaymentId))
                   .FirstOrDefault();

                if (tmp == null)
                {
                    var _model = await _converterHelper.ToOrdersTmpEntity(model, true);
                    _model.OrderStatus = _combo.Text;
                    _dataContext.prOrderDetailTmps.Add(_model);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    tmp.Quantity += model.Quantity;
                    _dataContext.prOrderDetailTmps.Update(tmp);
                    await _dataContext.SaveChangesAsync();
                }
                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Win",
                };
            }
            catch (SqlException sqlEx)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", sqlEx.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", sqlEx.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", sqlEx.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", sqlEx.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
        }

        public async Task<Response<object>> ConfirmOrderAsync(NewOrderModel model)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    var user = await _distributorHelper.GetByDistrEmailAsync(model.Debtor);
                    if (user == null)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "No distributor data check the information"
                        };
                    }

                    var orderTmps = await _dataContext.prOrderDetailTmps
                        .Where(o => o.Debtor == model.Debtor)
                        .ToListAsync();

                    if (orderTmps == null || orderTmps.Count == 0)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "There is no order data check the information!",
                        };
                    }
                    var _combo = _combosHelper.GetOrderStatuses().Where(s => s.Value == "2").FirstOrDefault();
                    var order = new PrOrders
                    {
                        OrderDate = DateTime.UtcNow,
                        Debtor = model.Debtor,
                        DistributorId = user.Result.DistributorId,
                        Observations = model.Observations,
                        OrderStatus = _combo.Text,
                        UserId = user.Result.UserId,
                        IsDeleted = 0,
                        DeliveryDate = Convert.ToDateTime("01/01/1900"),
                        CKamManagerId = user.Result.KamId,
                        GenerateUserId = user.Result.UserId,
                        RegistrationDate = DateTime.UtcNow,
                    };

                    _dataContext.PrOrders.Add(order);
                    await _dataContext.SaveChangesAsync();

                    var details = orderTmps.Select(o => new PrOrderDetails{
                        Price = o.Price,
                        Quantity = o.Quantity,
                        OrderId = order.OrderId,
                        DeatilStoreId = o.DeatilStoreId,
                        Observations = o.Observations,
                        OrderCode = o.OrderCode,
                        TaxRate = o.TaxRate,
                        OrderStatus = _combo.Text,
                        TypeofPaymentId = o.TypeofPaymentId,
                        GenerateDistributor = model.GenerateDistributor,
                        IsDeleted = 0,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                    }).ToList();

                    _dataContext.PrOrderDetails.AddRange(details);

                    _dataContext.prOrderDetailTmps.RemoveRange(orderTmps);
                    await _dataContext.SaveChangesAsync();
                    transaction.Commit();
                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = "Win",
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                    message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = message,
                    };
                }
            }
        }

        public Task DeleteDetailTempAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeliverOrder(DeliverViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<PrOrderDetailTmps>> GetDetailTempsAsync(string Debtor)
        {
            var _distr = await _distributorHelper.GetByDistrEmailAsync(Debtor);
            if (_distr.IsSuccess == false)
            {
                return null;
            }

            return _dataContext.prOrderDetailTmps
                .Include(o => o.DeatilWarehouses)
                .ThenInclude(wo => wo.Products)
                .Where(o => o.Debtor == _distr.Result.Debtor)
                .OrderBy(wo => wo.DeatilWarehouses.Products.Description);
        }

        public Task<IQueryable<PrOrders>> GetOrdersAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<PrOrders> GetOrdersAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, int quantity)
        {
            var orderDetailTemp = await _dataContext.prOrderDetailTmps.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity >= 150)
            {
                _dataContext.prOrderDetailTmps.Update(orderDetailTemp);
                await _dataContext.SaveChangesAsync();
            }
        }
        public async Task<List<OnlyOrderDetails>> GetOnlyOrdersAsync(long Distributorid)
        {

            try
            {
                List<OnlyOrderDetails> ListIndexOrder = await (from p in _dataContext.PrOrders
                                                               join u in _dataContext.Users on p.UserId equals u.UserId
                                                               join d in _dataContext.Distributors on p.DistributorId equals d.DistributorId
                                                               join k in _dataContext.Kams on d.KamId equals k.KamId
                                                               where d.DistributorId == Distributorid
                                                               select new OnlyOrderDetails
                                                               {
                                                                   Debtor = d.Debtor,
                                                                   BusinessName = d.BusinessName,
                                                                   Observations = p.Observations,
                                                                   OrderId = p.OrderId,
                                                                   FullName = $"{k.Users.FullName}",
                                                                   OrderDate = p.DateLocal,
                                                                   OrderStatus = p.OrderStatus,
                                                               }).ToListAsync();



                return ListIndexOrder.OrderBy(t => t.OrderId).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<OnlyOrderDetails>> GetCKOnlyOrdersAsync()
        {
            try
            {
                List<OnlyOrderDetails> ListIndexOrder = await (from p in _dataContext.PrOrders 
                                                               join u in _dataContext.Users on p.UserId equals u.UserId
                                                               join d in _dataContext.Distributors on p.DistributorId equals d.DistributorId
                                                               join k in _dataContext.Kams on d.KamId equals k.KamId
                                                               where p.GenerateUserId == 1
                                                               select new OnlyOrderDetails
                                                               {
                                                                   Debtor = d.Debtor,
                                                                   BusinessName = d.BusinessName,
                                                                   Observations = p.Observations,
                                                                   OrderId = p.OrderId,
                                                                   FullName = $"{k.Users.FullName}",
                                                                   OrderDate = p.DateLocal,
                                                                   OrderStatus = p.OrderStatus,
                                                               }).ToListAsync();



                return ListIndexOrder.OrderBy(t => t.OrderId).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> OlnyDetailOrder(long id) {
            try
            {
                var _detail = await (from d in _dataContext.PrOrderDetails
                               join dw in _dataContext.DeatilWarehouses
                               on d.DeatilStoreId equals dw.DeatilStoreId
                               join p in _dataContext.Products
                               on dw.ProductId equals p.ProductId
                               where d.OrderId.Equals(id)
                               select new {
                                   ShortDescription = p.ShortDescription,
                                   Description = p.Description,
                               }).FirstOrDefaultAsync();
                if (_detail == null)
                {
                    return "sin datos";
                }
                return _detail.ShortDescription;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<Response<object>> AddItemToGenerateaNormalOrderAsync(GenerateaNormalOrderViewModel model, string userName)
        {
            try
            {
                var _combo = _combosHelper
                    .GetOrderStatuses()
                    .Where(s => s.Value == "0")
                    .FirstOrDefault();

                var tmp = _dataContext.prOrderDetailTmps
                    .Where(odt => odt.DistributorId == model.DistributorId && odt.GenerateUserId == model.GenerateUserId
                    && odt.DeatilStoreId == model.DeatilStoreId && odt.TypeofPaymentId.Equals(model.TypeofPaymentId))
                    .FirstOrDefault();

                if (tmp == null)
                {
                    var _model = await _converterHelper.ToGenerateaNormalOrdersTmpEntity(model, true);
                    _model.OrderStatus = _combo.Text;
                    _dataContext.prOrderDetailTmps.Add(_model);
                    
                }
                else
                {
                    tmp.Quantity += model.Quantity;
                    _dataContext.prOrderDetailTmps.Update(tmp);
                }
                await _dataContext.SaveChangesAsync();
                return new Response<object>{
                    IsSuccess = true,
                    Message = "El pedido ya fue registro  y evaluando. ...!",
                };
            }
            catch (SqlException sqlEx)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", sqlEx.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", sqlEx.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", sqlEx.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", sqlEx.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
        }
        public async Task<Response<object>> SentKDOrderAsync(AddGenerateNormalOrderModel collection)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    var Kuser = await _dataContext.Kams
                                    .Include(u => u.Users)
                                    .Where(x => x.UserId == collection.UserId)
                                    .FirstOrDefaultAsync();
                    if (Kuser == null){
                        return new Response<object>{
                            IsSuccess = false,
                            Message = "No kam data check the information"
                        };
                    }

                    var orderTmps = await _dataContext
                        .prOrderDetailTmps
                        .Where(o => o.GenerateUserId == Kuser.UserId)
                        .ToListAsync();

                    if (orderTmps == null || orderTmps.Count == 0)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "There is no order data check the information!",
                        };
                    }
                    var _combo = _combosHelper.GetOrderStatuses().Where(s => s.Value == "2").FirstOrDefault();
                    var order = new PrOrders
                    {
                        OrderDate = DateTime.UtcNow,
                       // Debtor = model.Debtor,
                       // DistributorId = user.Result.DistributorId,
                       // Observations = model.Observations,
                        OrderStatus = _combo.Text,
                       // UserId = user.Result.UserId,
                        IsDeleted = 0,
                        CKamManagerId = 0,
                        GenerateUserId = 0,
                        RegistrationDate = DateTime.UtcNow,
                    };

                    _dataContext.PrOrders.Add(order);
                    await _dataContext.SaveChangesAsync();

                    var details = orderTmps.Select(o => new PrOrderDetails
                    {
                        Price = o.Price,
                        Quantity = o.Quantity,
                        OrderId = order.OrderId,
                        DeatilStoreId = o.DeatilStoreId,
                        Observations = o.Observations,
                        OrderCode = o.OrderCode,
                        TaxRate = o.TaxRate,
                        OrderStatus = _combo.Text,
                        TypeofPaymentId = o.TypeofPaymentId,
                    }).ToList();

                    _dataContext.PrOrderDetails.AddRange(details);

                    _dataContext.prOrderDetailTmps.RemoveRange(orderTmps);
                    await _dataContext.SaveChangesAsync();
                    transaction.Commit();
                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = "Win",
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                    message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = message,
                    };
                }
            }
        }

        public async Task<Response<object>> AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            try{
                var _combo = _combosHelper
                    .GetOrderStatuses()
                    .Where(s => s.Value == "0")
                    .FirstOrDefault();

                var tmp = _dataContext.prOrderDetailTmps
                   .Where(odt => odt.Debtor == model.Debtor.ToString()
                   && odt.DeatilStoreId == model.DeatilStoreId && odt.TypeofPaymentId.Equals(model.TypeofPaymentId))
                   .FirstOrDefault();

                if (tmp == null)
                {
                    var _model = await _converterHelper.ToOrdersTmpEntity(model, true);
                    _model.OrderStatus = _combo.Text;
                    _dataContext.prOrderDetailTmps.Add(_model);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    tmp.Quantity += model.Quantity;
                    _dataContext.prOrderDetailTmps.Update(tmp);
                    await _dataContext.SaveChangesAsync();
                }
                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Win",
                };
            }
            catch (SqlException sqlEx)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", sqlEx.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", sqlEx.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", sqlEx.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", sqlEx.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
        }

        public async Task<Response<object>> AddItemToGenerateaNormalOrderAsync(AddGenerateNormalOrderModel model, string userName)
        {
            try
            {
                var _combo = _combosHelper
                    .GetOrderStatuses()
                    .Where(s => s.Value == "0")
                    .FirstOrDefault();

                var tmp = _dataContext.prOrderDetailTmps
                    .Where(odt => odt.DistributorId == model.DistributorId && odt.GenerateUserId == model.GenerateUserId
                    && odt.DeatilStoreId == model.DeatilStoreId && odt.TypeofPaymentId.Equals(model.TypeofPaymentId))
                    .FirstOrDefault();

                if (tmp == null)
                {
                    var _model = await _converterHelper.ToGenerateaNormalOrdersTmpEntity(model, true);
                    _model.OrderStatus = _combo.Text;
                    _dataContext.prOrderDetailTmps.Add(_model);

                }
                else
                {
                    tmp.Quantity += model.Quantity;
                    _dataContext.prOrderDetailTmps.Update(tmp);
                }
                await _dataContext.SaveChangesAsync();
                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "El pedido ya fue registro y evaluando. ...!",
                };
            }
            catch (SqlException sqlEx)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", sqlEx.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", sqlEx.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", sqlEx.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", sqlEx.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
        }

        public async Task<Response<object>> AddToOrderGenerateNormalAsync(OrderVerificationViewModel model, string userName)
        {
            using (var transaction = _dataContext.Database.BeginTransaction()) {
                try{

                    var _user = await _dataContext
                         .Users
                         .Where(u => u.UserName.Equals(userName) && u.IsDeleted == 0 && u.IsDistributor == 0)
                         .FirstOrDefaultAsync();
                    if (_user == null)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "There is no registration of the requesting user",
                        };
                    }
                    var _distributor = await _dataContext
                        .Distributors
                        .Where(d => d.DistributorId == model.DistributorId && d.IsDeleted == 0)
                        .FirstOrDefaultAsync();

                    if (_distributor == null)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "There is no record of the requesting Distributor",
                        };
                    }

                    var orderTmps = await _dataContext.prOrderDetailTmps
                      .Where(o => o.DistributorId == _distributor.DistributorId && o.GenerateUserId == _user.UserId)
                      .ToListAsync();

                    if (orderTmps == null || orderTmps.Count == 0)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "There is no order Incentive data check the information!",
                        };
                    }
                    var _combo = _combosHelper.GetOrderStatuses().Where(s => s.Value == "2").FirstOrDefault();
                    var order = new PrOrders{
                        OrderDate = DateTime.UtcNow,
                        Debtor = _distributor.Debtor,
                        DistributorId = _distributor.DistributorId,
                        Observations = model.Observations,
                        OrderStatus = _combo.Text,
                        UserId = _user.UserId,
                        IsDeleted = 2,
                        RegistrationDate = DateTime.UtcNow,
                        DeliveryDate = Convert.ToDateTime("01/01/1900"),
                        CKamManagerId = model.KamId,
                        //GenerateUserId = Convert.ToInt32(_user.UserId),
                        GenerateUserId = 1,
                    };

                    _dataContext.PrOrders.Add(order);
                    await _dataContext.SaveChangesAsync();

                    var details = orderTmps.Select(o => new PrOrderDetails
                    {
                        Price = o.Price,
                        Quantity = o.Quantity,
                        OrderId = order.OrderId,
                        DeatilStoreId = o.DeatilStoreId,
                        OrderCode = o.OrderCode,
                        TaxRate = o.TaxRate,
                        TypeofPaymentId = o.TypeofPaymentId,
                        OrderStatus = _combo.Text,
                        IsDeleted = 0,
                        RegistrationDate = DateTime.UtcNow,
                    }).ToList();

                    _dataContext.PrOrderDetails.AddRange(details);

                    _dataContext.prOrderDetailTmps.RemoveRange(orderTmps);

                    await _dataContext.SaveChangesAsync();
                    transaction.Commit();
                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = "Win Order Generate Normal  !",
                    };
                }
                catch (Exception ex){
                    transaction.Rollback();
                    string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                    message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = message,
                    };
                }
            }
        }

        public async Task<Response<object>> AddItemToDNormalOrderAsync(AddItemViewModel model, string userName)
        {
            try
            {
                var _combo = _combosHelper
                    .GetOrderStatuses()
                    .Where(s => s.Value == "0")
                    .FirstOrDefault();

                var tmp = _dataContext.prOrderDetailTmps
                   .Where(odt => odt.Debtor == model.Debtor.ToString()
                   && odt.DeatilStoreId == model.DeatilStoreId && odt.TypeofPaymentId.Equals(model.TypeofPaymentId))
                   .FirstOrDefault();

                if (tmp == null)
                {
                    var _model = await _converterHelper.ToOrdersTmpEntity(model, true);
                    _model.OrderStatus = _combo.Text;
                    _dataContext.prOrderDetailTmps.Add(_model);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    tmp.Quantity += model.Quantity;
                    _dataContext.prOrderDetailTmps.Update(tmp);
                    await _dataContext.SaveChangesAsync();
                }
                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Win",
                };
            }
            catch (SqlException sqlEx)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", sqlEx.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", sqlEx.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", sqlEx.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", sqlEx.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = message,
                };
            }
        }
    }
}
