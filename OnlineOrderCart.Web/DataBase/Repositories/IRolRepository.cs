using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IRolRepository : IGenericRepository<Roles>
    {
        public Task<List<Roles>> GetRolesAsync();
        public Task<Roles> GetOnlyRolAsync(int id);
    }
}
