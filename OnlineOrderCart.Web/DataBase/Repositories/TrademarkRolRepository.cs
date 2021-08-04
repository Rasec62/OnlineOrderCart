using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class TrademarkRolRepository : GenericRepository<Trademarks>, ITrademarkRolRepository
    {
        private readonly DataContext _dataContext;

        public TrademarkRolRepository(DataContext context) : base(context)
        {
           _dataContext = context;
        }

        public async Task<Trademarks> GetOnlyTrademarkAsync(int id)
        {
           return await _dataContext.Trademarks.FindAsync(id);
        }

        public async Task<List<Trademarks>> GetAllRecordsAsync()
        {
            return await _dataContext
                .Trademarks
                .Where(t => t.IsDeleted == 0)
                .OrderByDescending(t => t.TrademarkId)
                .ToListAsync();
        }
    }
}
