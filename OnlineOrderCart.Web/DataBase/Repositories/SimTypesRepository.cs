using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class SimTypesRepository : GenericRepository<SimTypes>, ISimTypesRepository
    {
        private readonly DataContext _dataContext;

        public SimTypesRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task<List<SimTypes>> GetAllRecordsAsync()
        {
            return await _dataContext.SimTypes.Where(s => s.IsDeleted ==0).ToListAsync();
        }

        public async Task<SimTypes> GetOnlySimTypesAsync(int id)
        {
            return await _dataContext.SimTypes.FindAsync(id);
        }
    }
}
