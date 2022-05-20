using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Application.Interfaces;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Application.RepositoryHelpers
{
    public class IncentiveOrderDetailTmpRepository : IIncentiveOrderDetailTmpRepository
    {
        private readonly DataContext _dataContext;

        public IncentiveOrderDetailTmpRepository(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        public Task<IncentiveOrderDetailTmp> CreateAsync(IncentiveOrderDetailTmp entity)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(IncentiveOrderDetailTmp entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IncentiveOrderDetailTmp>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<IncUserOrdersVModel>> GetAllIncentiveOrderRecordsAsync(long UserId)
        {
            List<IncUserOrdersVModel> ListIndexIncentive = new List<IncUserOrdersVModel>();
            long UserCoordId = 0;
            try
            {
                var _KamCoord = await _dataContext.Kams
                               .Include(u => u.Users)
                               .Where(kc => kc.Users.UserId.Equals(UserId) && kc.IsDeleted.Equals(0) && kc.Users.IsDistributor.Equals(0))
                               .FirstOrDefaultAsync();

                if (_KamCoord == null)
                {
                    return new List<IncUserOrdersVModel>();
                }
                switch (_KamCoord.IsCoordinator)
                {
                    case 0:
                        var _Coord = await _dataContext.Kams
                            .Include(u => u.Users)
                            .Where(kc => kc.KamManagerId.Equals(_KamCoord.KamId) && kc.IsDeleted.Equals(0) && kc.Users.IsDistributor.Equals(0) && kc.IsCoordinator.Equals(1))
                            .FirstOrDefaultAsync();
                        if (_Coord != null)
                        {
                            if (_Coord.IsCoordinator == 1)
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


                ListIndexIncentive = await(from t in _dataContext.IncentiveOrders
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
            catch (Exception)
            {
                return new List<IncUserOrdersVModel>();
            }
        }

        public Task<IncentiveOrderDetailTmp> GetAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync()>0;
        }

        public Task<IncentiveOrderDetailTmp> UpdateAsync(IncentiveOrderDetailTmp entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
