namespace Eras.Domain.Repositories
{
    public interface IStudentRepository<T>
    {
        void Add(T student);
    }
}