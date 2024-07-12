using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
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
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task PatchAsync(int id, JsonPatchDocument<T> patchDoc)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Entity not found");
        }

        patchDoc.ApplyTo(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Entity not found");
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> ReadByQueryAsync(string sqlQuery, params object[] parameters)
    {
        return await _dbSet.FromSqlRaw(sqlQuery, parameters).ToListAsync();
    }

    public async Task WriteQueryAsync(string sqlQuery, params object[] parameters)
    {
        await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
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
