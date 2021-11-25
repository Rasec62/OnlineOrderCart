using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> Get();
        Task<TEntity> GetAsync(int id);
        void Add(TEntity data);
        void Delete(int id);
        void Update(TEntity data);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        void Save();
    }
}
