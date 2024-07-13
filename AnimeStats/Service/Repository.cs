using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AnimeStats.Service
{
    public abstract class Repository<T> where T : class
    {
        private readonly DatabaseEFCore db;
        private readonly DbSet<T> _dbSet;

        public Repository(DatabaseEFCore database)
        {
            db = database;
            _dbSet = database.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task PatchAsync(int id, JsonPatchDocument<T> patchDoc)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found");
            }

            patchDoc.ApplyTo(entity);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found");
            }

            _dbSet.Remove(entity);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> ReadByQueryAsync(string sqlQuery, params object[] parameters)
        {
            return await _dbSet.FromSqlRaw(sqlQuery, parameters).ToListAsync();
        }

        public async Task WriteQueryAsync(string sqlQuery, params object[] parameters)
        {
            await db.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            await db.SaveChangesAsync();
        }
    }

    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task PatchAsync(int id, JsonPatchDocument<T> patchDoc);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> ReadByQueryAsync(string sqlQuery, params object[] parameters);
        Task WriteQueryAsync(string sqlQuery, params object[] parameters);
    }
}