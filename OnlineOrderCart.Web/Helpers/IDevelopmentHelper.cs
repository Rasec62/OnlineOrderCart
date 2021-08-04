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
    }
}
