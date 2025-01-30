using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.Repositories
{
    //public class StudentRepository : IStudentRepository<Student>
    //{
    //    private readonly AppDbContext _context;

    //    public StudentRepository(AppDbContext context)
    //    {
    //        _context = context;
    //    }
    //    public async Task<Student> GetStudentByEmail(string email)
    //    {
    //        var studentByName = await _context.Students.Where(s => s.Email.Equals(email)).ToListAsync();             
    //        if (studentByName.Count > 0) return studentByName[0].ToStudent();
    //        return null;
    //    }
    //    public async Task<Student> Add(Student student)
    //    {
    //        var existingStudent = await GetStudentByEmail(student.Email);
    //        if (existingStudent != null) return existingStudent;

    //        var studentEntity = student.ToStudentEntity();
    //        _context.Students.Add(studentEntity);
    //        await _context.SaveChangesAsync();
    //        return studentEntity.ToStudent();
    //    }
    //}
}
