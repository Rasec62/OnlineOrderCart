using OnlineOrderCart.Common.Entities;
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

        Task AddItemToOrderAsync(AddItemViewModel model, string userName);

        Task ModifyOrderDetailTempQuantityAsync(int id, int quantity);

        Task DeleteDetailTempAsync(int id);

        Task<bool> ConfirmOrderAsync(string userName);

        Task DeliverOrder(DeliverViewModel model);
        Task<List<OnlyOrderDetails>> GetOnlyOrdersAsync(long Distributorid);
        Task<List<OnlyOrderDetails>> GetCKOnlyOrdersAsync();
    }
}
