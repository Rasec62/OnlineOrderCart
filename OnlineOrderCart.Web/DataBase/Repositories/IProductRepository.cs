using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IProductRepository : IGenericRepository<Products>
    {
        public Task<List<Products>> GetAllRecordsAsync();
        public Task<Products> GetOnlyProductAsync(int id);
    }
}
