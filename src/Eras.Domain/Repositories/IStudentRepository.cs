namespace Eras.Domain.Repositories
{
    public interface IStudentRepository<T>
    {
        Task<T> Add(T student);
    }
}