
using CargoHub.Api.Data;
using CargoHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoHub.Api.Repositories;

public class Repository<T> : IRepository<T>
    where T : class, IEntity
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<T>> GetAllWithDetailsAsync(
        params string[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdWithDetailsAsync(
        int id,
        params string[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
