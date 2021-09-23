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
        public async Task<List<OnlyOrderDetails>> GetOnlyOrdersAsync(long Distributorid)
        {

            try
            {
                List<OnlyOrderDetails> ListIndexOrder = await (from p in _datacontext.PrOrders
                                                               join u in _datacontext.Users on p.UserId equals u.UserId
                                                               join d in _datacontext.Distributors on p.DistributorId equals d.DistributorId
                                                               join k in _datacontext.Kams on d.KamId equals k.KamId
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
                List<OnlyOrderDetails> ListIndexOrder = await(from p in _datacontext.PrOrders
                                                              join u in _datacontext.Users on p.UserId equals u.UserId
                                                              join d in _datacontext.Distributors on p.DistributorId equals d.DistributorId
                                                              join k in _datacontext.Kams on d.KamId equals k.KamId
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
    }
}
