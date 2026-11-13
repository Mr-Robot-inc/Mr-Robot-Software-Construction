using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Tests.Unit.Fakes;

public class FakeRepository<T> : IRepository<T> where T : class, IEntity
{
    public List<T> Items { get; } = new();
    public int SaveCount { get; private set; }

    public Task<List<T>> GetAllAsync()
        => Task.FromResult(Items.ToList());

    public Task<T?> GetByIdAsync(int id)
        => Task.FromResult(Items.FirstOrDefault(p => p.Id == id));

    public void Add(T entity)
    {
        entity.Id = Items.Count + 1;
        Items.Add(entity);
    }

    public void Update(T entity) { }

    public void Delete(T entity) => Items.Remove(entity);

    public Task<int> SaveChangesAsync()
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}