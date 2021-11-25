using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _dbSet = _dataContext.Set<TEntity>();
        }
        public void Add(TEntity data) => _dbSet.Add(data);

        public void Delete(int id)
        {
            var dataDelete = _dbSet.Find(id);
            _dbSet.Remove(dataDelete);
        }

        public IEnumerable<TEntity> Get() => _dbSet.ToList();

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
      
        public async Task<TEntity>  GetAsync(int id) => await _dbSet.FindAsync(id);
        public void Update(TEntity data)
        {
            _dbSet.Attach(data);
            _dataContext.Entry(data).State = EntityState.Modified;
        }
        public void Save() => _dataContext.SaveChanges();

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            await SaveAllAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbSet.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
            await SaveAllAsync();
            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await SaveAllAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await this._dataContext.SaveChangesAsync() > 0;
        }
    }
}
