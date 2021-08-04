using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IActivationsFormRepository : IGenericRepository<ActivationsForm>
    {
        public Task<List<ActivationsForm>> GetAllRecordsAsync();
        public Task<ActivationsForm> GetOnlyActivationsFormAsync(int id);
    }
}
