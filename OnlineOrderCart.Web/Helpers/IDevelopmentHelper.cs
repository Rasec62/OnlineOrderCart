using OnlineOrderCart.Common.Responses;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IDevelopmentHelper
    {
        Task<List<TmpOrderViewModel>> GetSqlDataTmpOrders(long id);
        Task<List<TmpOrderViewModel>> GetSqlDataTmpCKOrders(long id);
        Task<List<TmpIncentiveDViewModel>> GetSqlDataTmpSpecialistIncentiveOrders(string UserName,long id);
        Task<Response<DisUserOrdersVModel>> GetSqlOnlyOrderRecordsAsync(long Userid, long id);
        Task<GenericResponse<InOrderDetailViewModel>> GetSqlAllOrderDetailsRecordsAsync<T>(long? id);
        Task<List<DKCEmailDetails>> GetSqlEmailDistrs(string Debtor);
        Task<DKCEmailDetails> GetOnlySqlEmailDistrs(long id);
        Task<DKCEmailDetails> GetOnlySqlEmailKamCoordDistrs(long DistributorId);
        Task<GenericResponse<DKCEmailDetails>> GetSqlEmailKamCs(string Username);
        Task<List<ObjectAvatarViewModel>> GetSqlDataOrderDetailsVMl(long id);
        Task<List<OrdersLayoutModel>> GetSqlDataIncentiveOrdersVMl(long id);
        Task<List<TmpIncentiveDViewModel>> GetSqlDataTmpOrdersIncentive(long userid);
    }
}
