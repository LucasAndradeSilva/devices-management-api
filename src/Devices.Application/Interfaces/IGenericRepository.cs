using System.Linq.Expressions;

namespace Devices.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);

    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        int page = 1,
        int pageSize = 10,
        bool asNoTracking = true);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task<int> SaveChangesAsync();
}