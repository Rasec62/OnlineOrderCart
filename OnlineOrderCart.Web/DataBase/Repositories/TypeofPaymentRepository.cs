using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class TypeofPaymentRepository : GenericRepository<TypeofPayments>, ITypeofPaymentRepository
    {
        private readonly DataContext _dataContext;

        public TypeofPaymentRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task<List<TypeofPayments>> GetAllTypeofPaymentsAsync()
        {
           return await _dataContext.TypeofPayments.Where(tp => tp.IsDeleted==0).ToListAsync();
        }

        public async Task<TypeofPayments> GetOnlyTypeofPaymentAsync(int id)
        {
            return await _dataContext.TypeofPayments.FirstOrDefaultAsync(tp => tp.TypeofPaymentId.Equals(id));
        }
    }
}
