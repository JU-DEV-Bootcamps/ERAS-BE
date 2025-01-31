namespace Eras.Domain.Repositories
{
    public interface IRepository<T>
    {
        Task<T> Add(T entity);
        Task<T> GetPaged(int page, int pageSize);
        Task<T> GetById(int id);
        Task<T> Delete(int id);
        Task<T> Update(T entity);   
    }
}