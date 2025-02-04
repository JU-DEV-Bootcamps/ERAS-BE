using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Cohort> Cohorts { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollInstance> PollInstances { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentDetail> StudentDetails { get; set; }
        public DbSet<Variable> Variables { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
    }
}
