using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class OrderIncentiveRepository : GenericRepository<IncentiveOrderDetailTmp>, IOrderIncentiveRepository
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;
        private readonly ICombosHelper _combosHelper;
        private List<TmpIncentiveDViewModel> ObjSpecialist = new List<TmpIncentiveDViewModel>();
        private List<TmpIncentiveDViewModel> ListObjSpecialistIncentiveOrders = new List<TmpIncentiveDViewModel>();
        public OrderIncentiveRepository(DataContext dataContext,
            IConverterHelper converterHelper, ICombosHelper combosHelper) : base(dataContext)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
            _combosHelper = combosHelper;
        }

        public async Task<List<IncentiveOrderDetailTmp>> GetAllRecordsAsync()
        {
            return await _dataContext
                .IncentiveOrderDetailTmp
                .OrderByDescending(a => a.IncentiveId)
                .ToListAsync();
        }
        public async Task<List<IndexIncentiveDViewModel>> GetAllIndexRecordsAsync(long KamId)
        {

            try
            {
                List<IndexIncentiveDViewModel> ListIndexIncentive = await (from t in _dataContext.IncentiveOrderDetails
                                                                           join io in _dataContext.IncentiveOrders on t.IncentiveOrderId equals io.IncentiveOrderId
                                                                           join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                                                           join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                                                           join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                                                           join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                                                           join d in _dataContext.Distributors on io.Debtor equals d.Debtor
                                                                           join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                                                           where d.KamId == KamId
                                                                           select new IndexIncentiveDViewModel
                                                                           {
                                                                               Debtor = io.Debtor,
                                                                               BusinessName = d.BusinessName,
                                                                               Observations = io.Observations,
                                                                               DeatilStoreId = t.DeatilStoreId,
                                                                               IncentiveOrderId = io.IncentiveOrderId,
                                                                               Price = t.Price,
                                                                               Quantity = t.Quantity,
                                                                               TaxPrice = t.TaxPrice,
                                                                               OrderCode = t.OrderCode,
                                                                               OraclepId = p.OraclepId,
                                                                               OrderStatus = t.OrderStatus,
                                                                               DeatilProducts = p.Description,
                                                                               ShippingBranchNo = wr.ShippingBranchNo,
                                                                               ShippingBranchName = wr.ShippingBranchName,
                                                                               MD = d.MD.ToUpper(),
                                                                               PayofType = pay.PaymentName,

                                                                           }).ToListAsync();



                return ListIndexIncentive.OrderBy(t => t.IncentiveOrderId).ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<List<IncUserOrdersVModel>> GetAllIncentiveOrderRecordsAsync(long UserId)
        {
            List<IncUserOrdersVModel> ListIndexIncentive = new List<IncUserOrdersVModel>();
            long UserCoordId = 0;
            try
            {
                var _KamCoord = await _dataContext.Kams
                               .Include(u => u.Users)
                               .Where(kc => kc.Users.UserId.Equals(UserId)&& kc.IsDeleted.Equals(0)&& kc.Users.IsDistributor.Equals(0))
                               .FirstOrDefaultAsync();

                if (_KamCoord == null)
                {
                    return new List<IncUserOrdersVModel>();
                }
                switch (_KamCoord.IsCoordinator) {
                    case 0:
                        var _Coord = await _dataContext.Kams
                            .Include(u => u.Users)
                            .Where(kc => kc.KamManagerId.Equals(_KamCoord.KamId) && kc.IsDeleted.Equals(0) && kc.Users.IsDistributor.Equals(0) && kc.IsCoordinator.Equals(1))
                            .FirstOrDefaultAsync();
                        if (_Coord != null)
                        {
                            if (_Coord.IsCoordinator ==1)
                            {
                                UserCoordId = _Coord.UserId;
                            }
                        }
                        break;
                    case 1:
                        var _kam = await _dataContext.Kams
                            .Include(u => u.Users)
                            .Where(kc => kc.KamId.Equals(_KamCoord.KamManagerId) && kc.IsDeleted.Equals(0) && kc.Users.IsDistributor.Equals(0) && kc.IsCoordinator.Equals(0))
                            .FirstOrDefaultAsync();
                        if (_kam.IsCoordinator == 0)
                        {
                            UserCoordId = _kam.UserId;
                        }
                        break;
                }


                ListIndexIncentive = await (from t in _dataContext.IncentiveOrders
                                        join u in _dataContext.Users on t.UserId equals u.UserId
                                        join d in _dataContext.Distributors on t.DistributorId equals d.DistributorId
                                        where t.UserId.Equals(_KamCoord.UserId) || t.UserId.Equals(UserCoordId)
                                        select new IncUserOrdersVModel
                                        {
                                            Debtor = d.Debtor,
                                            BusinessName = d.BusinessName,
                                            Observations = t.Observations,
                                            IncentiveOrderId = t.IncentiveOrderId,
                                            FirstName = u.FirstName,
                                            LastName1 = u.LastName1,
                                            LastName2 = u.LastName2,
                                            OrderDate = t.OrderDate,
                                            OrderStatus = t.OrderStatus,
                                        }).ToListAsync();



                return ListIndexIncentive.OrderBy(t => t.IncentiveOrderId).ToList();
            }
            catch (Exception){
                return new List<IncUserOrdersVModel>();
            }
        }
        public async Task ModifyOrderIncentiveDetailTempQuantityAsync(int id, int quantity)
        {
            try
            {
                var orderDetailTemp = await _dataContext
                    .IncentiveOrderDetailTmp
                    .Where(t => t.IncentiveId.Equals(id))
                    .FirstOrDefaultAsync();
                if (orderDetailTemp == null)
                {
                    return;
                }

                orderDetailTemp.Quantity += quantity;
                if (orderDetailTemp.Quantity >= 50)
                {
                    _dataContext.IncentiveOrderDetailTmp.Update(orderDetailTemp);
                    await _dataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Response<object>> AddItemToOrderIncentiveAsync(NOrderIncentiveViewModel model, string userName)
        {
            try
            {
                var _user = await _dataContext
                         .Kams
                         .Include(u => u.Users)
                         .Where(u => u.Users.UserName.Equals(userName) && u.IsDeleted == 0)
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
                var _product = await _dataContext.DeatilWarehouses
                   .Include(p => p.Products)
                   .Where(p => p.DeatilStoreId.Equals(model.DeatilStoreId) && p.IsDeleted == 0 && p.Products.IsDeleted == 0)
                   .FirstOrDefaultAsync();
                if (_product == null)
                {
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = "the product does not exist in the system",
                    };
                }

                var Itmp = await _dataContext
                    .IncentiveOrderDetailTmp
                    .Where(odt => odt.Debtor == _distributor.Debtor
                    && odt.DeatilStoreId == _product.DeatilStoreId
                    && odt.UserId == _user.Users.UserId).FirstOrDefaultAsync();

                if (Itmp == null)
                {
                    model.Debtor = _distributor.Debtor;
                    model.Price = Convert.ToDouble(_product.Products.Price);
                    model.TaxPrice = _product.Products.ValueWithOutTax;
                    var _model = _converterHelper.ToIncentiveOrdersTmpEntity(model, true);
                    _model.UserId = _user.UserId;
                    _dataContext.IncentiveOrderDetailTmp.Add(_model);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    Itmp.Quantity += model.Quantity;
                    _dataContext.IncentiveOrderDetailTmp.Update(Itmp);
                    await _dataContext.SaveChangesAsync();
                }
                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "successful operation with registration!",
                };
            }
            catch (System.Exception ex)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response<object>> AddToOrderIncentiveAsync(DeliverOrderIncentiveViewModel model, string userName)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
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
                    SpecialistDetails(model.DistributorId, _distributor.Debtor);

                    var orderTmps = await _dataContext.IncentiveOrderDetailTmp
                       .Where(o => o.DistributorId == _distributor.DistributorId)
                       .ToListAsync();

                    if (orderTmps == null || orderTmps.Count == 0)
                    {
                        return new Response<object>
                        {
                            IsSuccess = false,
                            Message = "There is no order Incentive data check the information!",
                        };
                    }
                    var _combo = _combosHelper.GetOrderStatuses()
                        .Where(s => s.Value == "2").FirstOrDefault();

                    var order = new IncentiveOrders{
                        OrderDate = DateTime.UtcNow,
                        Debtor = _distributor.Debtor,
                        DistributorId = _distributor.DistributorId,
                        Observations = model.Observations,
                        OrderStatus = _combo.Text,
                        UserId = _user.UserId,
                        IsDeleted = 2,
                        RegistrationDate = DateTime.UtcNow,
                    };

                    _dataContext.IncentiveOrders.Add(order);
                    await _dataContext.SaveChangesAsync();

                    var details = orderTmps.Select(o => new IncentiveOrderDetails{
                        Price = o.Price,
                        Quantity = o.Quantity,
                        IncentiveOrderId = order.IncentiveOrderId,
                        DeatilStoreId = o.DeatilStoreId,
                        OrderCode = o.OrderCode,
                        TaxPrice = o.TaxPrice,
                        TypeofPaymentId = o.TypeofPaymentId,
                        OrderStatus = _combo.Text,
                        IsDeleted = 0,
                        RegistrationDate = DateTime.UtcNow,
                    }).ToList();

                    _dataContext.IncentiveOrderDetails.AddRange(details);

                    //var date = DateTime.ParseExact("01-12-2001", "dd/MM/yyyy hh:mm",CultureInfo.InvariantCulture);
                    //var date1 = DateTime.ParseExact("01-12-2001", "MM/dd/yyyy hh:mm", CultureInfo.InvariantCulture);

                    _dataContext.IncentiveOrderDetailTmp.RemoveRange(orderTmps);
                    await _dataContext.SaveChangesAsync();
                    transaction.Commit();
                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = "Win Incentive Order",
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sbII = new StringBuilder();
                    sbII.Append($"{"<b>Message:</b> {0}<br /><br />"}{ex.Message}");
                    sbII.Append($"{"<b>StackTrace:</b> {0}<br /><br />"}{ex.StackTrace.Replace(Environment.NewLine, string.Empty)}");
                    sbII.Append($"{"<b>Source:</b> {0}<br /><br />"}{ex.Source.Replace(Environment.NewLine, string.Empty)}");
                    sbII.Append($"{"<b>TargetSite:</b> {0}"}{ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty)}");
                    
                    string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                    message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                    message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                    sb.Append(message);
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = sbII.ToString(),
                    };
                }
            }
        }
        public void SpecialistDetails(long distributorId, string UserName)
        {

            //Creating List
        
            var _UseAvatar = _dataContext.Kams
                    .Include(k => k.Users)
                    .Where(u => u.Users.UserName == UserName)
                    .FirstOrDefault();

            if (_UseAvatar == null)
            {
                return;
            }
            ObjSpecialist = (from t in _dataContext.IncentiveOrderDetailTmp
                             join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                             join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                             join p in _dataContext.Products on dw.ProductId equals p.ProductId
                             join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                             join d in _dataContext.Distributors on t.Debtor equals d.Debtor
                             join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                             where t.UserId == _UseAvatar.UserId || t.DistributorId == distributorId
                             select new TmpIncentiveDViewModel{
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

                             }).OrderBy(x => x.IncentiveId).ToList();

            int recordsTotal = ObjSpecialist.Count;
            int count = 1;

            foreach (var itemIncen in ObjSpecialist.OrderBy(x => x.IncentiveId).ToList()){
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
                    PayofType = itemIncen.PayofType,
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
        }
        public async Task<IncUserOrdersVModel> GetOnlyIncentiveOrderRecordsAsync(long UserId, long IncentiveOrderId)
        {
            try
            {
                IncUserOrdersVModel ListIndexIncentive = await (from t in _dataContext.IncentiveOrders
                                                                join u in _dataContext.Users on t.UserId equals u.UserId
                                                                join d in _dataContext.Distributors on t.DistributorId equals d.DistributorId
                                                                where t.UserId == UserId || t.IncentiveOrderId == IncentiveOrderId
                                                                select new IncUserOrdersVModel{
                                                                    Debtor = d.Debtor,
                                                                    BusinessName = d.BusinessName,
                                                                    Observations = t.Observations,
                                                                    IncentiveOrderId = t.IncentiveOrderId,
                                                                    FirstName = u.FirstName,
                                                                    LastName1 = u.LastName1,
                                                                    LastName2 = u.LastName2,
                                                                    OrderDate = t.OrderDate,
                                                                    OrderStatus = t.OrderStatus,
                                                                }).FirstOrDefaultAsync();



                return ListIndexIncentive;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<IndexIncentiveDViewModel>> GetAllIncDetailRecordsAsync(long IncentiveOrderId)
        {
            List<IndexIncentiveDViewModel> ListIndexIncentive = new List<IndexIncentiveDViewModel>();
            try
            {
               ListIndexIncentive = await (from t in _dataContext.IncentiveOrderDetails
                                        join io in _dataContext.IncentiveOrders on t.IncentiveOrderId equals io.IncentiveOrderId
                                        join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                        join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                        join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                        join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                        join d in _dataContext.Distributors on io.Debtor equals d.Debtor
                                        join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                        where io.IncentiveOrderId == IncentiveOrderId
                                        select new IndexIncentiveDViewModel
                                        {
                                            Debtor = io.Debtor,
                                            BusinessName = d.BusinessName,
                                            Observations = io.Observations,
                                            DeatilStoreId = t.DeatilStoreId,
                                            IncentiveOrderId = io.IncentiveOrderId,
                                            Price = t.Price,
                                            Quantity = t.Quantity,
                                            TaxPrice = t.TaxPrice,
                                            OrderCode = t.OrderCode,
                                            OraclepId = p.OraclepId,
                                            OrderStatus = t.OrderStatus,
                                            DeatilProducts = p.Description,
                                            ShippingBranchNo = wr.ShippingBranchNo,
                                            ShippingBranchName = wr.ShippingBranchName,
                                            MD = d.MD.ToUpper(),
                                            PayofType = pay.PaymentName,

                                        }).ToListAsync();



                return ListIndexIncentive.OrderBy(t => t.IncentiveOrderId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
