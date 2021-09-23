using Microsoft.EntityFrameworkCore;
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

        public async Task<List<TmpOrderViewModel>> GetSqlDataTmpCKOrders()
        {
            try
            {
                List<TmpOrderViewModel> ListTmp = await(from t in _dataContext.prOrderDetailTmps
                                                        join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                                        join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                                        join p in _dataContext.Products on dw.ProductId equals p.ProductId
                                                        join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                                        join d in _dataContext.Distributors on t.Debtor equals d.Debtor
                                                        where t.GenerateUserId > 0
                                                        select new TmpOrderViewModel
                                                        {
                                                            Debtor = t.Debtor,
                                                            Observations = t.Observations,
                                                            DeatilStoreId = t.DeatilStoreId,
                                                            OrderDetailTmpId = t.OrderDetailTmpId,
                                                            Price = t.Price,
                                                            Quantity = t.Quantity,
                                                            TaxRate = t.TaxRate,
                                                            OrderStatus = t.OrderStatus,
                                                            DeatilProducts = p.Description,
                                                            OrderCode = $"{d.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{"-"}{wr.ShippingBranchNo}{p.OraclepId}{ma.Description}{" "}{"Pay of Type"}{" "}{string.Format("{0:C4}", t.Quantity * t.Price)}"

                                                        }).ToListAsync();
                return ListTmp.OrderBy(t => t.OrderDetailTmpId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TmpOrderViewModel>> GetSqlDataTmpOrders(string Debtor)
        {
            try{
                List<TmpOrderViewModel> ListTmp = await (from t  in _dataContext.prOrderDetailTmps
                                                         join dw in _dataContext.DeatilWarehouses on t.DeatilStoreId equals dw.DeatilStoreId
                                                         join wr in _dataContext.Warehouses on dw.StoreId equals wr.StoreId
                                                         join p  in _dataContext.Products on dw.ProductId equals p.ProductId
                                                         join ma in _dataContext.Trademarks on p.TrademarkId equals ma.TrademarkId
                                                         join d  in _dataContext.Distributors on t.Debtor equals d.Debtor
                                                         where t.Debtor.Equals(Debtor)
                                                         select new TmpOrderViewModel{
                                                               Debtor = t.Debtor,
                                                               Observations = t.Observations,
                                                               DeatilStoreId = t.DeatilStoreId,
                                                               OrderDetailTmpId = t.OrderDetailTmpId,
                                                               Price = t.Price,
                                                               Quantity = t.Quantity,
                                                               TaxRate = t.TaxRate,
                                                               OrderStatus = t.OrderStatus,
                                                               DeatilProducts = p.Description,
                                                               OrderCode = $"{d.MD}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Year.ToString("D4")}{"-"}{wr.ShippingBranchNo}{p.OraclepId}{ma.Description}{" "}{"Pay of Type"}{" "}{string.Format("{0:C4}",t.Quantity * t.Price)}"

                                                         }).ToListAsync();
                return ListTmp.OrderBy(t=>t.OrderDetailTmpId).ToList();
            }
            catch (Exception ex){
                return null;
            }
        }
    }
}
