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

        public async Task<List<TmpOrderViewModel>> GetSqlDataTmpOrders(string Debtor)
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
                                                         where t.Debtor.Equals(Debtor)
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
            catch (Exception){
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
            try
            {
                var _d = await (from d in _dataContext.Distributors
                                join u in _dataContext.Users on d.UserId equals u.UserId
                                where d.Debtor == Debtor
                                select new
                                {
                                    KamId = d.KamId,
                                    DEmail = u.Email,
                                }).FirstOrDefaultAsync();

                var _dkc = await (from k in _dataContext.Kams
                                  join u in _dataContext.Users on k.UserId equals u.UserId
                                  join c in _dataContext.Kams on k.KamId equals c.KamManagerId
                                  where k.KamId == _d.KamId
                                  select new DKCEmailDetails
                                  {
                                      EmailK = k.Users.Email,
                                      EmailC = c.Users.Email,
                                      EmailD = _d.DEmail,
                                  }).ToListAsync();

                if (_dkc.Count > 0)
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
                                 }).ToListAsync();
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
    }
}
