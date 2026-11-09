using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities.UserManagement;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.UserManagement;

public sealed class ProfessionalProfileRepository(AppDbContext Context) : IProfessionalProfileRepository
{
    private readonly AppDbContext _context = Context;

    public async Task<ProfessionalProfile?> GetByErasUserIdAsync(int ErasUserId)
    {
        ProfessionalProfile? profile = await _context.ProfessionalProfiles
            .AsNoTracking()
            .Include(Saved => Saved.Courses)
            .FirstOrDefaultAsync(Saved => Saved.ErasUserId == ErasUserId);

        if (profile is not null)
            profile.Courses = profile.Courses.OrderBy(Course => Course.Id).ToList();

        return profile;
    }

    public async Task<ProfessionalProfile> SaveAsync(ProfessionalProfile Profile)
    {
        if (Profile.Id == 0)
        {
            _context.ProfessionalProfiles.Add(Profile);
            await _context.SaveChangesAsync();
            return Profile;
        }

        ProfessionalProfile existing = await _context.ProfessionalProfiles
            .Include(Saved => Saved.Courses)
            .FirstAsync(Saved => Saved.Id == Profile.Id);

        existing.Faculty = Profile.Faculty;
        existing.OfficeHours = Profile.OfficeHours;
        existing.SubjectsTaught = Profile.SubjectsTaught;
        existing.Audit = Profile.Audit;

        // The saved courses are replaced as a whole: removed ones are deleted, the rest are inserted.
        existing.Courses.Clear();
        foreach (ProfessionalCourse course in Profile.Courses)
        {
            existing.Courses.Add(new ProfessionalCourse { Code = course.Code, Name = course.Name });
        }

        await _context.SaveChangesAsync();
        return existing;
    }
}
