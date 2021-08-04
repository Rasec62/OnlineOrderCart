using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class ActivationsTypeRepository : GenericRepository<ActivationsType>, IActivationsTypeRepository
    {
        private readonly DataContext _dataContext;

        public ActivationsTypeRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }
        public async Task<List<ActivationsType>> GetAllRecordsAsync()
        {
           return await _dataContext.ActivationsType.Where(a => a.IsDeleted ==0).ToListAsync();
        }

        public async Task<ActivationsType> GetOnlyActivationsTypeAsync(int id)
        {
           return await _dataContext
                .ActivationsType
                .Where(a => a.ActivationTypeId == id)
                .FirstOrDefaultAsync();
        }
    }
}
