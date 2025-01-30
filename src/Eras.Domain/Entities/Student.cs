namespace Eras.Domain.Entities
{
    public class Student
    {
        public string Uuid { get; set; } = default!;

        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public StudentDetail? StudentDetail { get; set; }

    }
}
