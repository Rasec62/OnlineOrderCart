using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class ProductsTypeRespository : GenericRepository<ProductsType>, IProductsTypeRespository
    {
        private readonly DataContext _dataContext;

        public ProductsTypeRespository(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task<List<ProductsType>> GetAllRecordsAsync()
        {
            return await _dataContext.ProductsType.Where(p => p.IsDeleted == 0).ToListAsync();
        }

        public async Task<ProductsType> GetOnlyProductsTypeAsync(int id)
        {
            return await _dataContext.ProductsType.FindAsync(id);
        }
    }
}
