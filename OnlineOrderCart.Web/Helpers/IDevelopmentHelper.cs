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
        Task<List<TmpOrderViewModel>> GetSqlDataTmpOrders(string Debtor);
        Task<List<TmpOrderViewModel>> GetSqlDataTmpCKOrders(long id);
        Task<Response<DisUserOrdersVModel>> GetSqlOnlyOrderRecordsAsync(long Userid, long id);
        Task<GenericResponse<InOrderDetailViewModel>> GetSqlAllOrderDetailsRecordsAsync<T>(long? id);
        Task<List<DKCEmailDetails>> GetSqlEmailDistrs(string Debtor);
        Task<GenericResponse<DKCEmailDetails>> GetSqlEmailKamCs(string Username);
    }
}
