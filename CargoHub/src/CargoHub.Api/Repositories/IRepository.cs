
using CargoHub.Api.Models;

namespace CargoHub.Api.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);

    Task<List<T>> GetAllWithDetailsAsync(
        params string[] includes);

    Task<T?> GetByIdWithDetailsAsync(
        int id,
        params string[] includes);

    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task<int> SaveChangesAsync();
}