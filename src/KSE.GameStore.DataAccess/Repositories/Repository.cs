using System.Linq.Expressions;
using KSE.GameStore.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly GameStoreDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public Repository(GameStoreDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    
    public async Task<IEnumerable<T>> ListAsync(int? pageNumber = 1, int? pageSize = 10)
    {
        IQueryable<T> query = _dbSet;
        
        var safePageNumber = pageNumber ?? 1;
        var safePageSize = pageSize ?? 10;
        
        var skip = (safePageNumber - 1) * safePageSize;
        query = query.Skip(skip).Take(safePageSize);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, int? pageNumber = 1, int? pageSize = 10)
    {
        var query = _dbSet.Where(predicate);
        
        var safePageNumber = pageNumber ?? 1;
        var safePageSize = pageSize ?? 10;

        var skip = (safePageNumber - 1) * safePageSize;
        query = query.Skip(skip).Take(safePageSize);

        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}