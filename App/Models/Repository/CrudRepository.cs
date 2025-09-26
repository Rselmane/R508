using App.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

public abstract class CrudRepository<TEntity> : IDataRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;

    public CrudRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public abstract Task<TEntity?> GetByStringAsync(string str);

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity; 
    }

    public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
    {
        _context.Set<TEntity>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}