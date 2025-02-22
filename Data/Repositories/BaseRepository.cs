using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

/// <summary>
/// Abstract class that implements basic CRUD operations.
/// </summary>
public abstract class BaseRepository<TEntity>(DataContext context) : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly DataContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();


    // ===========================================
    //                  CREATE
    // ===========================================
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {    
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }


    // ===========================================
    //                   READ
    // ===========================================
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet.Where(expression).ToListAsync();
    }

    public virtual async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet.FirstOrDefaultAsync(expression);
    }


    // ===========================================
    //                 UPDATE
    // ===========================================
    public virtual async Task<TEntity?> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }


    // ===========================================
    //                 DELETE
    // ===========================================
    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}