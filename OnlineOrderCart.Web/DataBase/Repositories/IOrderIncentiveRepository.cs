using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IOrderIncentiveRepository : IGenericRepository<IncentiveOrderDetailTmp>
    {
       Task<List<IncentiveOrderDetailTmp>> GetAllRecordsAsync();
       Task<List<IndexIncentiveDViewModel>> GetAllIndexRecordsAsync(long KamId);
       Task<List<IncUserOrdersVModel>> GetAllIncentiveOrderRecordsAsync(long UserId);
       Task ModifyOrderIncentiveDetailTempQuantityAsync(int id, int quantity);
       Task<Response<object>> AddItemToOrderIncentiveAsync(NOrderIncentiveViewModel model, string userName);
       Task<Response<object>> AddToOrderIncentiveAsync(DeliverOrderIncentiveViewModel model, string userName);
       Task<IncUserOrdersVModel> GetOnlyIncentiveOrderRecordsAsync(long UserId, long IncentiveOrderId);
       Task<List<IndexIncentiveDViewModel>> GetAllIncDetailRecordsAsync(long IncentiveOrderId);
    }
}
