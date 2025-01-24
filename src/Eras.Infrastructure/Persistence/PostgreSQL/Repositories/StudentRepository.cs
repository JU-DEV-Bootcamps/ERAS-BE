using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

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
            var efStudent = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (efStudent == null)
                return null;

            return MapToDomain(efStudent);
        }

        public async Task<Student?> GetByUuidAsync(string uuid)
        {
            var efStudent = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.Uuid == uuid);

            if (efStudent == null)
                return null;

            return MapToDomain(efStudent);
        }

        public async Task SaveAsync(Student domainStudent)
        {
            var efStudent = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.Uuid == domainStudent.Uuid);

            if (efStudent == null)
            {
                efStudent = new Students();
                _context.Students.Add(efStudent);
            }
            efStudent.Uuid = domainStudent.Uuid;
            efStudent.Name = domainStudent.Name;
            efStudent.Email = domainStudent.Email;
            efStudent.ModifiedDate = System.DateTime.UtcNow;

            if (efStudent.StudentDetails == null)
            {
                efStudent.StudentDetails = new StudentDetails();
            }
            
            if (domainStudent.StudentDetail != null)
            {
                efStudent.StudentDetails.EnrolledCourses = domainStudent.StudentDetail.EnrolledCourses;
                efStudent.StudentDetails.GradedCourses = domainStudent.StudentDetail.GradedCourses;
                efStudent.StudentDetails.TimeDeliveryRate = domainStudent.StudentDetail.TimeDeliveryRate;
                efStudent.StudentDetails.AvgScore = domainStudent.StudentDetail.AvgScore;
                efStudent.StudentDetails.CoursesUnderAvg = domainStudent.StudentDetail.CoursesUnderAvg;
                efStudent.StudentDetails.PureScoreDiff = domainStudent.StudentDetail.PureScoreDiff;
                efStudent.StudentDetails.StandardScoreDiff = domainStudent.StudentDetail.StandardScoreDiff;
                efStudent.StudentDetails.LastAccessDays = domainStudent.StudentDetail.LastAccessDays;
                efStudent.StudentDetails.ModifiedDate = System.DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string uuid)
        {
            var efStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Uuid == uuid);
            if (efStudent != null)
            {
                _context.Students.Remove(efStudent);
                await _context.SaveChangesAsync();
            }
        }

        private Student MapToDomain(Students efStudent)
        {
            var domainStudent = new Student
            {
                Uuid = efStudent.Uuid ?? string.Empty,
                Name = efStudent.Name,
                Email = efStudent.Email
            };

            if (efStudent.StudentDetails != null)
            {
                domainStudent.StudentDetail = new StudentDetail
                {
                    EnrolledCourses = efStudent.StudentDetails.EnrolledCourses,
                    GradedCourses = efStudent.StudentDetails.GradedCourses,
                    TimeDeliveryRate = efStudent.StudentDetails.TimeDeliveryRate,
                    AvgScore = efStudent.StudentDetails.AvgScore,
                    CoursesUnderAvg = efStudent.StudentDetails.CoursesUnderAvg,
                    PureScoreDiff = efStudent.StudentDetails.PureScoreDiff,
                    StandardScoreDiff = efStudent.StudentDetails.StandardScoreDiff,
                    LastAccessDays = efStudent.StudentDetails.LastAccessDays
                };
            }

            return domainStudent;
        }
    }
}
