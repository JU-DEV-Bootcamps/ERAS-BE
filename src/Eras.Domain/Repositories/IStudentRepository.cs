namespace Eras.Domain.Repositories
{
    public interface IStudentRepository<T>
    {
        Task Add(T student);
    }
}