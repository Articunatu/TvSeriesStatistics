using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AnimeStats.Service
{
    public class Repository<T>(DatabaseEFCore database) where T : class
    {
        private readonly DatabaseEFCore mssql = database;
        private readonly DbSet<T> db = database.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await db.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await db.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await db.AddAsync(entity);
            await mssql.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            db.Attach(entity);
            mssql.Entry(entity).State = EntityState.Modified;
            await mssql.SaveChangesAsync();
        }

        public async Task PatchAsync(int id, JsonPatchDocument<T> patchDoc)
        {
            var entity = await db.FindAsync(id) ?? throw new KeyNotFoundException("Entity not found");
            patchDoc.ApplyTo(entity);
            await mssql.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await db.FindAsync(id) ?? throw new KeyNotFoundException("Entity not found");
            db.Remove(entity);
            await mssql.SaveChangesAsync();
        }
        //where 1=1 to extend queries easier
        public async Task<IEnumerable<T>> ReadByQueryAsync(string sqlQuery, params object[] parameters)
        {
            return await db.FromSqlRaw(sqlQuery, parameters).ToListAsync();
        }

        public async Task WriteQueryAsync(string sqlQuery, params object[] parameters)
        {
            await mssql.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            await mssql.SaveChangesAsync();
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