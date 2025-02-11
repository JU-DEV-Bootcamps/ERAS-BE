using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class AppDbContext : DbContext
    {
        public DbSet<AnswerEntity> Answers { get; set; }
        public DbSet<CohortEntity> Cohorts { get; set; }
        public DbSet<ComponentEntity> Components { get; set; }
        public DbSet<PollEntity> Polls { get; set; }
        public DbSet<PollInstanceEntity> PollInstances { get; set; }
        public DbSet<PollVariableMapping> PollVariables { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<StudentDetailEntity> StudentDetails { get; set; }
        public DbSet<VariableEntity> Variables { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
