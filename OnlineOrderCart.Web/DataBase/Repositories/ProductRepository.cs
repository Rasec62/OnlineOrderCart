using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class ProductRepository : GenericRepository<Products>, IProductRepository
    {
        private readonly DataContext _dataContext;

        public ProductRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task<List<Products>> GetAllRecordsAsync()
        {
            return await _dataContext.Products
                .Include(p => p.ProductsType)
                .Include(p => p.Trademarks)
                .Include(p => p.SimTypes)
                .Include(p => p.ActivationsForm)
                .Include(p => p.ActivationsType)
                .Where(p => p.IsDeleted == 0).OrderByDescending(p => p.ProductId).ToListAsync();
        }

        public async Task<Products> GetOnlyProductAsync(int id)
        {
            return await _dataContext
                .Products
                .Include(p => p.ProductsType)
                .Include(p => p.Trademarks)
                .Include(p => p.SimTypes)
                .Include(p => p.ActivationsForm)
                .Include(p => p.ActivationsType)
                .Where(p => p.ProductId == id).FirstOrDefaultAsync();
        }
    }
}
