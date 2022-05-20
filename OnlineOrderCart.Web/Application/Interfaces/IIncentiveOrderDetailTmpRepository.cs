using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Application.Interfaces
{
    public interface IIncentiveOrderDetailTmpRepository : IFactoryRepository<IncentiveOrderDetailTmp>
    {
        Task<List<IncUserOrdersVModel>> GetAllIncentiveOrderRecordsAsync(long UserId);
    }
}
