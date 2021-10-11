using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public class DevelopmentHelper : IDevelopmentHelper
    {
        private readonly DataContext _dataContext;
        private List<TmpIncentiveDViewModel> ObjSpecialist = new List<TmpIncentiveDViewModel>();
        private List<TmpIncentiveDViewModel> ListObjSpecialistIncentiveOrders = new List<TmpIncentiveDViewModel>();
        public DevelopmentHelper(DataContext context){
            _dataContext = context;
        }
        public async Task<List<TmpOrderViewModel>> GetSqlDataTmpCKOrders(long id)
        {
            try
            {
                List<TmpOrderViewModel> ListTmp = await (from t in _dataContext.prOrderDetailTmps
                                                         join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                                         join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                                         join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                                         join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                                         join d in _dataContext.Distributors on t.DistributorId equals d.DistributorId
                                                         join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                                         where t.GenerateUserId == id
                                                         select new TmpOrderViewModel
                                                         {
                                                             Debtor = t.Debtor,
                                                             Observations = t.Observations,
                                                             DeatilStoreId = t.DeatilStoreId,
                                                             OrderDetailTmpId = t.OrderDetailTmpId,
                                                             Price = t.Price,
                                                             Quantity = t.Quantity,
                                                             TaxRate = t.TaxRate,
                                                             OrderCode = p.ShortDescription,
                                                             OrderStatus = t.OrderStatus,
                                                             DeatilProducts = p.Description,
                                                             ShippingBranchNo = wr.ShippingBranchNo,
                                                             MD = d.MD.ToUpper(),
                                                             PayofType = pay.PaymentName,
                                                         }).ToListAsync();

                int _counts = ListTmp.Count;
                int count = 1;
                List<TmpOrderViewModel> ListTmpOrders = new List<TmpOrderViewModel>();
                foreach (var item in ListTmp.ToList())
                {
                    var tmpo = new TmpOrderViewModel
                    {
                        Debtor = item.Debtor,
                        Observations = item.Observations,
                        DeatilStoreId = item.DeatilStoreId,
                        OrderDetailTmpId = item.OrderDetailTmpId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TaxRate = item.TaxRate,
                        OrderStatus = item.OrderStatus,
                        DeatilProducts = item.DeatilProducts,
                        OrderCode = $"{item.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{" - "}{item.ShippingBranchNo}{" - "}{item.OrderCode}{" - "}{item.PayofType}{" - "}{string.Format("{0:C2}", item.Quantity * item.Price)}{" - "}{count}{" de "}{_counts}",
                    };
                    var _Pord = await _dataContext.prOrderDetailTmps.Where(p => p.OrderDetailTmpId == tmpo.OrderDetailTmpId).FirstOrDefaultAsync();
                    _Pord.OrderCode = tmpo.OrderCode;
                    _dataContext.prOrderDetailTmps.Update(_Pord);
                    await _dataContext.SaveChangesAsync();
                    ListTmpOrders.Add(tmpo);
                    count++;
                }

                return ListTmpOrders.OrderBy(t => t.OrderDetailTmpId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"{"No show to Warehouse List"} { ex.Message}");
            }
        }
        public async Task<List<TmpOrderViewModel>> GetSqlDataTmpOrders(long id)
        {
            try
            {
                List<TmpOrderViewModel> ListTmp = await (from t in _dataContext.prOrderDetailTmps
                                                         join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                                         join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                                         join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                                         join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                                         join d in _dataContext.Distributors on t.Debtor equals d.Debtor
                                                         join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                                         where t.DistributorId.Equals(id)
                                                         select new TmpOrderViewModel
                                                         {
                                                             Debtor = t.Debtor,
                                                             Observations = t.Observations,
                                                             DeatilStoreId = t.DeatilStoreId,
                                                             OrderDetailTmpId = t.OrderDetailTmpId,
                                                             Price = t.Price,
                                                             Quantity = t.Quantity,
                                                             TaxRate = t.TaxRate,
                                                             OrderCode = p.ShortDescription,
                                                             OrderStatus = t.OrderStatus,
                                                             DeatilProducts = p.Description,
                                                             ShippingBranchNo = wr.ShippingBranchNo,
                                                             MD = d.MD.ToUpper(),
                                                             PayofType = pay.PaymentName,
                                                             ShortDescription = p.ShortDescription,
                                                             OrderDate = t.DateLocal,
                                                         }).ToListAsync();

                int _counts = ListTmp.Count;
                int count = 1;
                List<TmpOrderViewModel> ListTmpOrders = new List<TmpOrderViewModel>();
                foreach (var item in ListTmp.ToList())
                {
                    var tmpo = new TmpOrderViewModel
                    {
                        Debtor = item.Debtor,
                        Observations = item.Observations,
                        DeatilStoreId = item.DeatilStoreId,
                        OrderDetailTmpId = item.OrderDetailTmpId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TaxRate = item.TaxRate,
                        OrderStatus = item.OrderStatus,
                        DeatilProducts = item.DeatilProducts,
                        OrderDate = item.OrderDate,
                        ShortDescription = item.ShortDescription,
                        OrderCode = $"{item.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{" - "}{item.ShippingBranchNo}{" - "}{item.OrderCode}{" - "}{item.PayofType}{" - "}{string.Format("{0:C2}", item.Quantity * item.Price)}{" - "}{count}{" de "}{_counts}",
                    };
                    var _Pord = await _dataContext.prOrderDetailTmps.Where(p => p.OrderDetailTmpId == tmpo.OrderDetailTmpId).FirstOrDefaultAsync();
                    _Pord.OrderCode = tmpo.OrderCode;
                    _dataContext.prOrderDetailTmps.Update(_Pord);
                    await _dataContext.SaveChangesAsync();
                    ListTmpOrders.Add(tmpo);
                    count++;
                }

                return ListTmpOrders.OrderBy(t => t.OrderDetailTmpId).ToList();
            }
            catch (Exception ex){
                return null;
            }
        }
        public async Task<Response<DisUserOrdersVModel>> GetSqlOnlyOrderRecordsAsync(long Userid, long id)
        {
            DisUserOrdersVModel ListIndexOrder = new DisUserOrdersVModel();
            try{
                 ListIndexOrder = await(from t in _dataContext.PrOrders
                                    join u in _dataContext.Users on t.UserId equals u.UserId
                                    join d in _dataContext.Distributors on t.DistributorId equals d.DistributorId
                                    join k in _dataContext.Kams on d.KamId equals k.KamId
                                        //where t.GenerateUserId == Userid && t.OrderId == id
                                    where t.OrderId == id
                                    select new DisUserOrdersVModel
                                    {
                                        Debtor = d.Debtor,
                                        BusinessName = d.BusinessName,
                                        Observations = t.Observations,
                                        OrderId = t.OrderId,
                                        FirstName = k.Users.FirstName,
                                        LastName1 = k.Users.LastName1,
                                        LastName2 = k.Users.LastName2,
                                        OrderDate = t.OrderDate,
                                        OrderStatus = t.OrderStatus,
                                    }).FirstOrDefaultAsync();

                return new Response<DisUserOrdersVModel>
                {
                    IsSuccess = true,
                    Result = ListIndexOrder,
                }; ;
            }
            catch (Exception ex){
                return new Response<DisUserOrdersVModel> {
                     IsSuccess = false,
                     Message = ex.Message,
                };
            }
        }
        public async Task<GenericResponse<InOrderDetailViewModel>> GetSqlAllOrderDetailsRecordsAsync<T>(long? id)
        {
            List<InOrderDetailViewModel> ListIndexOrderDetails = new List<InOrderDetailViewModel>(); 
            try
            {
                if (id == null)
                {
                    return new GenericResponse<InOrderDetailViewModel>
                    {
                        IsSuccess = false,
                        Message = "There is not Data !",
                    };
                }
                 ListIndexOrderDetails = await(from t in _dataContext.PrOrderDetails
                                            join io in _dataContext.PrOrders on t.OrderId equals io.OrderId
                                            join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                            join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                            join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                            join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                            join d in _dataContext.Distributors on io.Debtor equals d.Debtor
                                            join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                            where io.OrderId == id
                                            select new InOrderDetailViewModel
                                            {
                                                Debtor = io.Debtor,
                                                BusinessName = d.BusinessName,
                                                Observations = io.Observations,
                                                DeatilStoreId = t.DeatilStoreId,
                                                OrderId = io.OrderId,
                                                OrderDate = io.OrderDate.ToLocalTime(),
                                                DeliveryDate = io.DeliveryDate,
                                                Price = t.Price,
                                                Quantity = t.Quantity,
                                                TaxPrice = t.TaxRate,
                                                OrderCode = t.OrderCode,
                                                OraclepId = p.OraclepId,
                                                OrderStatus = t.OrderStatus,
                                                DeatilProducts = p.Description,
                                                ShippingBranchNo = wr.ShippingBranchNo,
                                                ShippingBranchName = wr.ShippingBranchName,
                                                MD = d.MD.ToUpper(),
                                                PayofType = pay.PaymentName,
                                                ShortDescription = p.ShortDescription,

                                            }).ToListAsync();



                return new GenericResponse<InOrderDetailViewModel>
                {
                    IsSuccess = true,
                    ListResults = ListIndexOrderDetails,
                };
            }
            catch (Exception exs)
            {
                return new GenericResponse<InOrderDetailViewModel>
                {
                    IsSuccess = false,
                    Message = exs.Message,
                };
            }
        }
        public async Task<List<DKCEmailDetails>> GetSqlEmailDistrs(string Debtor)
        {
            List<OptionalEmailModel> OptionalEmailDetails = new List<OptionalEmailModel>();
            try
            {
                var _d = await (from d in _dataContext.Distributors
                                join u in _dataContext.Users on d.UserId equals u.UserId
                                where d.Debtor == Debtor
                                select new
                                {
                                    KamId = d.KamId,
                                    DEmail = u.Email,
                                    UserId = u.UserId,
                                }).FirstOrDefaultAsync();

                var _dop = await _dataContext.TblOptionalEmail
                    .Where(x => x.UserId == _d.UserId && x.Debtor == Debtor).ToListAsync();

                if (_dop != null){
                    OptionalEmailDetails = _dop.Select(o => new OptionalEmailModel{ 
                      OptionalEmail = o.OptionalEmail,
                     }).ToList();
                }

                var _dkc = await (from k in _dataContext.Kams
                                  join u in _dataContext.Users on k.UserId equals u.UserId
                                  join c in _dataContext.Kams on k.KamId equals c.KamManagerId
                                  where k.KamId == _d.KamId
                                  select new DKCEmailDetails
                                  {
                                      EmailK = k.Users.Email,
                                      EmailC = c.Users.Email,
                                      EmailD = _d.DEmail,
                                     OptionalEmailDetails = OptionalEmailDetails,
                                  }).ToListAsync();

                if (_dkc.Count > 0){
                    return _dkc;
                }

                var _dk = await (from k in _dataContext.Kams
                                 join u in _dataContext.Users on k.UserId equals u.UserId
                                 where k.KamId == _d.KamId
                                 select new DKCEmailDetails
                                 {
                                     EmailK = k.Users.Email,
                                     EmailD = _d.DEmail,
                                     OptionalEmailDetails = OptionalEmailDetails,
                                 }).ToListAsync();
                return _dk;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<DKCEmailDetails> GetOnlySqlEmailDistrs(long id)
        {
            List<OptionalEmailModel> OptionalEmailDetails = new List<OptionalEmailModel>();
            try
            {
                var _d = await (from d in _dataContext.Distributors
                                join u in _dataContext.Users on d.UserId equals u.UserId
                                where d.DistributorId == id && d.IsDeleted == 0 && u.IsDistributor == 1
                                select new{
                                    KamId = d.KamId,
                                    DEmail = u.Email,
                                    UserId = u.UserId,
                                    Debtor = d.Debtor,
                                }).FirstOrDefaultAsync();

                var _dop = await _dataContext.TblOptionalEmail
                    .Where(x => x.UserId == _d.UserId && x.Debtor == _d.Debtor)
                    .ToListAsync();

                if (_dop != null)
                {
                    OptionalEmailDetails = _dop.Select(o => new OptionalEmailModel
                    {
                        OptionalEmail = o.OptionalEmail,
                    }).ToList();
                }

                var _dkc = await (from k in _dataContext.Kams
                                  join u in _dataContext.Users on k.UserId equals u.UserId
                                  join c in _dataContext.Kams on k.KamId equals c.KamManagerId
                                  where k.KamId == _d.KamId
                                  select new DKCEmailDetails
                                  {
                                      EmailK = k.Users.Email,
                                      EmailC = c.Users.Email,
                                      EmailD = _d.DEmail,
                                      OptionalEmailDetails = OptionalEmailDetails,
                                  }).FirstOrDefaultAsync();

                if (_dkc!= null)
                {
                    return _dkc;
                }

                var _dk = await (from k in _dataContext.Kams
                                 join u in _dataContext.Users on k.UserId equals u.UserId
                                 where k.KamId == _d.KamId
                                 select new DKCEmailDetails
                                 {
                                     EmailK = k.Users.Email,
                                     EmailD = _d.DEmail,
                                     OptionalEmailDetails = OptionalEmailDetails,
                                 }).FirstOrDefaultAsync();
                return _dk;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<GenericResponse<DKCEmailDetails>> GetSqlEmailKamCs(string Username)
        {
            try
            {
                var _kc = await (from k in _dataContext.Kams
                                join u in _dataContext.Users on k.UserId equals u.UserId
                                where u.UserName == Username && u.IsDeleted == 0 && u.IsDistributor == 0
                                select new
                                {
                                    KamId = k.KamId,
                                    DEmail = u.Email,
                                    KamManagerId = k.KamManagerId == null? 0: k.KamManagerId,
                                    IsCoordinator = k.IsCoordinator,
                                }).FirstOrDefaultAsync();

                //if (_kc.IsCoordinator==0 && _kc.KamManagerId == 0){
                    
                //    var _dk = await (from k in _dataContext.Kams
                //                     join u in _dataContext.Users on k.UserId equals u.UserId
                //                     where k.KamId == _d.KamId
                //                     select new DKCEmailDetails
                //                     {
                //                         EmailK = k.Users.Email,
                //                         EmailD = _d.DEmail,
                //                     }).ToListAsync();
                //}

                return new GenericResponse<DKCEmailDetails>{
                    IsSuccess = true,
                    Message = "",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<DKCEmailDetails> { 
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<List<ObjectAvatarViewModel>> GetSqlDataOrderDetailsVMl(long id)
        {
            List<ObjectAvatarViewModel> ListIndexOrder = new List<ObjectAvatarViewModel>();
            try
            {
             ListIndexOrder = await (from p in _dataContext.PrOrders
                                    join u in _dataContext.Users on p.UserId equals u.UserId
                                    join d in _dataContext.Distributors on p.DistributorId equals d.DistributorId
                                    join k in _dataContext.Kams on d.KamId equals k.KamId
                                    where p.DistributorId == id
                                    select new ObjectAvatarViewModel{
                                        Debtor = d.Debtor,
                                        BusinessName = d.BusinessName,
                                        Observations = p.Observations,
                                        OrderId = p.OrderId,
                                        OrderDate = p.OrderDate,
                                        OrderStatus = p.OrderStatus,
                                    }).ToListAsync();
                return ListIndexOrder.OrderBy(t => t.OrderId).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<OrdersLayoutModel>> GetSqlDataIncentiveOrdersVMl(long id)
        {
            List<OrdersLayoutModel> ListIndexIncentive = new List<OrdersLayoutModel>();
            try
            {
                ListIndexIncentive = await (from io in _dataContext.IncentiveOrders
                                            join d in _dataContext.Distributors on io.Debtor equals d.Debtor
                                            join u in _dataContext.Users on d.UserId equals u.UserId
                                            where d.DistributorId == id
                                            select new OrdersLayoutModel{
                                                Debtor = io.Debtor,
                                                BusinessName = d.BusinessName,
                                                IncentiveOrderId = io.IncentiveOrderId,
                                                OrderDate = io.OrderDate,
                                                OrderStatus = io.OrderStatus,
                                            }).ToListAsync();

                return ListIndexIncentive.OrderBy(t => t.IncentiveOrderId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<TmpIncentiveDViewModel>> GetSqlDataTmpOrdersIncentive(long userid)
        {
            List<TmpIncentiveDViewModel> ListTmp = new List<TmpIncentiveDViewModel>();
            List<TmpIncentiveDViewModel> ListTmpIncentiveOrders = new List<TmpIncentiveDViewModel>();
            try
            {
                long _UserCoorId = 0;
                var _KamCoord = await _dataContext.Kams
                                .Include(u => u.Users)
                                .Where(x => x.UserId.Equals(userid) && x.Users.IsDistributor == 0 && x.Users.IsDeleted == 0 && x.IsDeleted == 0)
                                .FirstOrDefaultAsync();

                if (_KamCoord == null){
                    return ListTmpIncentiveOrders;
                }

                if (_KamCoord.IsCoordinator == 0)
                {
                    var _CoordDist = await _dataContext.Kams
                               .Include(u => u.Users)
                               .Where(x => x.KamManagerId.Equals(userid) && x.IsCoordinator == 1 && x.Users.IsDistributor == 0 && x.Users.IsDeleted == 0 && x.IsDeleted == 0)
                               .FirstOrDefaultAsync();

                    if (_CoordDist != null)
                    {
                        _UserCoorId = _CoordDist.UserId;
                    }
                }

                ListTmp = await (from t in _dataContext.IncentiveOrderDetailTmp
                join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                join p in _dataContext.Products on dw.ProductId equals p.ProductId
                join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                join d in _dataContext.Distributors on t.DistributorId equals d.DistributorId
                join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                where t.UserId == _KamCoord.UserId || t.UserId == _UserCoorId
                select new TmpIncentiveDViewModel{
                    Debtor = t.Debtor,
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

                }).ToListAsync();

                int _counts = ListTmp.Count;
                int count = 1;
                
                foreach (var itemIncen in ListTmp)
                {
                    var tmpo = new TmpIncentiveDViewModel{
                        Debtor = itemIncen.Debtor,
                        Observations = itemIncen.Observations,
                        DeatilStoreId = itemIncen.DeatilStoreId,
                        IncentiveId = itemIncen.IncentiveId,
                        Price = itemIncen.Price,
                        Quantity = itemIncen.Quantity,
                        TaxPrice = itemIncen.TaxPrice,
                        OrderStatus = itemIncen.OrderStatus,
                        DeatilProducts = itemIncen.DeatilProducts,
                        OrderCode = $"{itemIncen.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{" - "}{itemIncen.ShippingBranchNo}{" - "}{itemIncen.OrderCode}{" - "}{itemIncen.PayofType}{" - "}{string.Format("{0:C2}", itemIncen.Quantity * itemIncen.Price)}{" - "}{count}{" de "}{_counts}",
                    };
                    var _tmpi = await _dataContext
                        .IncentiveOrderDetailTmp
                        .Where(i => i.IncentiveId == tmpo.IncentiveId)
                        .FirstOrDefaultAsync();
                    _tmpi.OrderCode = tmpo.OrderCode;
                    _dataContext.IncentiveOrderDetailTmp.Update(_tmpi);
                    await _dataContext.SaveChangesAsync();
                    ListTmpIncentiveOrders.Add(tmpo);
                    count++;
                }

                return ListTmpIncentiveOrders.OrderBy(t => t.IncentiveId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<TmpIncentiveDViewModel>> GetSqlDataTmpSpecialistIncentiveOrders(string UserName,long distributorId)
        {
            try
            {
                var _UseAvatar = await _dataContext.Kams
                    .Include(k => k.Users)
                    .Where(u => u.Users.UserName == UserName)
                    .FirstOrDefaultAsync();

                if (_UseAvatar == null)
                {
                    return ObjSpecialist;
                }
                ObjSpecialist = await(from t in _dataContext.IncentiveOrderDetailTmp
                                 join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                 join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                 join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                 join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                 join d in _dataContext.Distributors on t.Debtor equals d.Debtor
                                 join pay in _dataContext.TypeofPayments on t.TypeofPaymentId equals pay.TypeofPaymentId
                                 where d.KamId == _UseAvatar.KamId || d.DistributorId == distributorId
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

                                 }).ToListAsync();

                int recordsTotal = ObjSpecialist.Count;
                int count = 1;

                foreach (var itemIncen in ObjSpecialist){
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
                    return ListObjSpecialistIncentiveOrders;
                }
                //return list as Json    
                return ListObjSpecialistIncentiveOrders;
            }
            catch (Exception)
            {
                return ObjSpecialist;
            }
        }

        public async Task<DKCEmailDetails> GetOnlySqlEmailKamCoordDistrs(long DistributorId)
        {
            List<OptionalEmailModel> OptionalEmailDetails = new List<OptionalEmailModel>();
            try
            {
                var _d = await(from d in _dataContext.Distributors
                               join u in _dataContext.Users on d.UserId equals u.UserId
                               where d.DistributorId == DistributorId && d.IsDeleted == 0 && u.IsDistributor == 1
                               select new
                               {
                                   KamId = d.KamId,
                                   DEmail = u.Email,
                                   UserId = u.UserId,
                                   Debtor = d.Debtor,
                               }).FirstOrDefaultAsync();

                var _dop = await _dataContext.TblOptionalEmail
                    .Where(x => x.UserId == _d.UserId && x.Debtor == _d.Debtor)
                    .ToListAsync();

                if (_dop != null && _dop.Count >0){
                    OptionalEmailDetails = _dop.Select(o => new OptionalEmailModel
                    {
                        OptionalEmail = o.OptionalEmail,
                    }).ToList();
                }

                var _dkc = await(from k in _dataContext.Kams
                                 join u in _dataContext.Users on k.UserId equals u.UserId
                                 join c in _dataContext.Kams on k.KamId equals c.KamManagerId
                                 where k.KamId == _d.KamId
                                 select new DKCEmailDetails
                                 {
                                     EmailK = k.Users.Email,
                                     EmailC = c.Users.Email,
                                     EmailD = _d.DEmail,
                                     OptionalEmailDetails = OptionalEmailDetails,
                                 }).FirstOrDefaultAsync();

                if (_dkc != null)
                {
                    return _dkc;
                }

                var _dk = await(from k in _dataContext.Kams
                                join u in _dataContext.Users on k.UserId equals u.UserId
                                where k.KamId == _d.KamId
                                select new DKCEmailDetails
                                {
                                    EmailK = k.Users.Email,
                                    EmailD = _d.DEmail,
                                    OptionalEmailDetails = OptionalEmailDetails,
                                }).FirstOrDefaultAsync();
                return _dk;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
