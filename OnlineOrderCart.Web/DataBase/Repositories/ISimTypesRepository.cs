using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface ISimTypesRepository : IGenericRepository<SimTypes>
    {
        public Task<List<SimTypes>> GetAllRecordsAsync();
        public Task<SimTypes> GetOnlySimTypesAsync(int id);
    }
}
