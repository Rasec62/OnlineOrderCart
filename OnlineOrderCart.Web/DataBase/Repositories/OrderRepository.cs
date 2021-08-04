using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
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
        private readonly DataContext _datacontext;
        private readonly IDistributorHelper _distributorHelper;

        public OrderRepository(DataContext datacontext,
            IDistributorHelper distributorHelper) : base(datacontext)
        {
            _datacontext = datacontext;
            _distributorHelper = distributorHelper;
        }

        public Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfirmOrderAsync(string userName)
        {
            throw new NotImplementedException();
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

            return _datacontext.prOrderDetailTmps
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
            var orderDetailTemp = await _datacontext.prOrderDetailTmps.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity >= 150)
            {
                _datacontext.prOrderDetailTmps.Update(orderDetailTemp);
                await _datacontext.SaveChangesAsync();
            }
        }
    }
}
