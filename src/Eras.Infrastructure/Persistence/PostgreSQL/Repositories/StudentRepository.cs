using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            var studentEntity = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            return studentEntity == null ? null : MapToDomain(studentEntity);
        }

        public async Task<Student?> GetByUuidAsync(string uuid)
        {
            var studentEntity = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.Uuid == uuid);

            return studentEntity == null ? null : MapToDomain(studentEntity);
        }

        public async Task SaveAsync(Student domainStudent)
        {
            var studentEntity = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.Uuid == domainStudent.Uuid);

            if (studentEntity == null)
            {
                studentEntity = new Students();
                _context.Students.Add(studentEntity);
            }

            studentEntity.Uuid = domainStudent.Uuid;
            studentEntity.Name = domainStudent.Name;
            studentEntity.Email = domainStudent.Email;
            studentEntity.ModifiedDate = System.DateTime.UtcNow;

            if (studentEntity.StudentDetails == null)
            {
                studentEntity.StudentDetails = new StudentDetails();
            }

            studentEntity.StudentDetails.EnrolledCourses = domainStudent.StudentDetail.EnrolledCourses;
            studentEntity.StudentDetails.GradedCourses = domainStudent.StudentDetail.GradedCourses;
            studentEntity.StudentDetails.TimeDeliveryRate = domainStudent.StudentDetail.TimeDeliveryRate;
            studentEntity.StudentDetails.AvgScore = domainStudent.StudentDetail.AvgScore;
            studentEntity.StudentDetails.CoursesUnderAvg = domainStudent.StudentDetail.CoursesUnderAvg;
            studentEntity.StudentDetails.PureScoreDiff = domainStudent.StudentDetail.PureScoreDiff;
            studentEntity.StudentDetails.StandardScoreDiff = domainStudent.StudentDetail.StandardScoreDiff;
            studentEntity.StudentDetails.LastAccessDays = domainStudent.StudentDetail.LastAccessDays;
            studentEntity.StudentDetails.ModifiedDate = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string uuid)
        {
            var studentEntity = await _context.Students
                .FirstOrDefaultAsync(s => s.Uuid == uuid);

            if (studentEntity != null)
            {
                _context.Students.Remove(studentEntity);
                await _context.SaveChangesAsync();
            }
        }

        private Student MapToDomain(Students studentEntity)
        {
            var domainStudent = new Student
            {
                Uuid = studentEntity.Uuid ?? string.Empty,
                Name = studentEntity.Name,
                Email = studentEntity.Email
            };

            if (studentEntity.StudentDetails != null)
            {
                domainStudent.StudentDetail = new StudentDetail
                {
                    EnrolledCourses = studentEntity.StudentDetails.EnrolledCourses,
                    GradedCourses = studentEntity.StudentDetails.GradedCourses,
                    TimeDeliveryRate = studentEntity.StudentDetails.TimeDeliveryRate,
                    AvgScore = studentEntity.StudentDetails.AvgScore,
                    CoursesUnderAvg = studentEntity.StudentDetails.CoursesUnderAvg,
                    PureScoreDiff = studentEntity.StudentDetails.PureScoreDiff,
                    StandardScoreDiff = studentEntity.StudentDetails.StandardScoreDiff,
                    LastAccessDays = studentEntity.StudentDetails.LastAccessDays
                };
            }

            return domainStudent;
        }
    }
}
