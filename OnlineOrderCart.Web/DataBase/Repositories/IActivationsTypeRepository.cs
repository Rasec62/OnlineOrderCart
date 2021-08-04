using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IActivationsTypeRepository : IGenericRepository<ActivationsType>
    {
        public Task<List<ActivationsType>> GetAllRecordsAsync();
        public Task<ActivationsType> GetOnlyActivationsTypeAsync(int id);
    }
}
