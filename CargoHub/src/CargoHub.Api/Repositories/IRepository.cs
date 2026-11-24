using CargoHub.Api.Models;
using System.Linq.Expressions;

namespace CargoHub.Api.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var all = await GetAllAsync();
        var matches = predicate.Compile();
        return all.Where(matches).ToList();
    }

    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task<int> SaveChangesAsync();
}