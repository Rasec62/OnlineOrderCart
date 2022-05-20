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
    public interface IOrderRepository : IGenericRepository<PrOrders>
    {
        Task<IQueryable<PrOrders>> GetOrdersAsync(string userName);

        Task<PrOrders> GetOrdersAsync(int id);

        Task<IQueryable<PrOrderDetailTmps>> GetDetailTempsAsync(string Debtor);

        Task<Response<object>> AddItemToOrderAsync(AddItemViewModel model, string userName);

        Task<Response<object>> AddItemToGenerateaNormalOrderAsync(AddGenerateNormalOrderModel model, string userName);

        Task ModifyOrderDetailTempQuantityAsync(int id, int quantity);

        Task DeleteDetailTempAsync(int id);

        Task<Response<object>> ConfirmOrderAsync(NewOrderModel model);

        Task<Response<object>> SentKDOrderAsync(AddGenerateNormalOrderModel collection);

        Task DeliverOrder(DeliverViewModel model);
        Task<List<OnlyOrderDetails>> GetOnlyOrdersAsync(long Distributorid);
        Task<List<OnlyOrderDetails>> GetCKOnlyOrdersAsync();
      
        Task<Response<object>> AddToOrderGenerateNormalAsync(OrderVerificationViewModel model, string userName);
        Task<Response<object>> AddItemToDNormalOrderAsync(AddItemViewModel model, string userName);
    }
}
