using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouses>, IWarehouseRepository
    {
        private readonly DataContext _dataContext;

        public WarehouseRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task<List<Warehouses>> GetAllRecordsAsync()
        {
           var ListWarehouse = await _dataContext.Warehouses
                               .Include(w => w.Distributors)
                               .Include(w => w.GetDeatilWarehouses)
                               .ThenInclude(wd => wd.Products)
                               .OrderBy(w => w.StoreId)
                               .ToListAsync();

            return ListWarehouse;
        }

        public async Task<Warehouses> GetOnlyWarehouseAsync(long id)
        {
            return await _dataContext.Warehouses.FindAsync(id);
        }
    }
}
