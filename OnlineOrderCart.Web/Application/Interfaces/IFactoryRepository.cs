using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Application.Interfaces
{
    public interface IFactoryRepository<TEntity> where TEntity : class
    {
        //IEnumerable<TEntity> Get();
        //void Add(TEntity data);
        //void Delete(int id);
        //void Update(TEntity data);
        //void Save();
        Task<TEntity> GetAsync(int id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<bool> SaveAllAsync();
    }
}
