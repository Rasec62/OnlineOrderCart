using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface ITypeofPaymentRepository : IGenericRepository<TypeofPayments>
    {
        public Task<List<TypeofPayments>> GetAllTypeofPaymentsAsync();
        public Task<TypeofPayments> GetOnlyTypeofPaymentAsync(int id);
    }
}
