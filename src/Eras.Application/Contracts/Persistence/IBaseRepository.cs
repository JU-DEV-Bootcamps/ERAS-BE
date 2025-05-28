namespace Eras.Application.Contracts.Persistence;
public interface IBaseRepository<T>
{
    Task<T> AddAsync(T Entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetPagedAsync(int Page, int PageSize);
    Task<T?> GetByIdAsync(int Id);
    Task DeleteAsync(T Entity);
    Task<T> UpdateAsync(T Entity);
    Task<int> CountAsync();

}
