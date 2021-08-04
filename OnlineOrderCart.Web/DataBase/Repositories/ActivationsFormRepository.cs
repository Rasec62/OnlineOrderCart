using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class ActivationsFormRepository : GenericRepository<ActivationsForm>, IActivationsFormRepository
    {
        private readonly DataContext _dataContext;

        public ActivationsFormRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }
        public async Task<List<ActivationsForm>> GetAllRecordsAsync()
        {
            return await _dataContext.ActivationsForm.Where(a => a.IsDeleted == 0).OrderByDescending(a => a.ActivationFormId).ToListAsync();
        }

        public async Task<ActivationsForm> GetOnlyActivationsFormAsync(int id)
        {
           return await _dataContext.ActivationsForm.FindAsync(id);
        }
    }
}
