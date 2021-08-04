using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class RolRepository : GenericRepository<Roles>, IRolRepository
    {
        private readonly DataContext _dataContext;

        public RolRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }
        public async Task<List<Roles>> GetRolesAsync()
        {
            return await _dataContext.Roles.Where(r => r.IsDeleted == 0).ToListAsync();
        }

        public async Task<Roles> GetOnlyRolAsync(int id)
        {
            return await _dataContext.Roles.Where(r => r.RolId==id).FirstOrDefaultAsync();
        }
    }
}
