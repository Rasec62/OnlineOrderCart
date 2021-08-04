using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface ITrademarkRolRepository : IGenericRepository<Trademarks>
    {
        public Task<List<Trademarks>> GetAllRecordsAsync();
        public Task<Trademarks> GetOnlyTrademarkAsync(int id);
    }
}
