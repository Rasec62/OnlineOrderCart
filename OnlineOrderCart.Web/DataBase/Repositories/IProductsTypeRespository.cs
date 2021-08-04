using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IProductsTypeRespository : IGenericRepository<ProductsType>
    {
        public Task<List<ProductsType>> GetAllRecordsAsync();
        public Task<ProductsType> GetOnlyProductsTypeAsync(int id);
    }
}
