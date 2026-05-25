namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IBaseRepository<T>
    where T : class
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();
}
