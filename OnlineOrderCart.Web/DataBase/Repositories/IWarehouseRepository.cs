using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IWarehouseRepository : IGenericRepository<Warehouses>
    {
        public Task<List<Warehouses>> GetAllRecordsAsync();
        public Task<Warehouses> GetOnlyWarehouseAsync(long id);
    }
}
