using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<AnswerEntity> Answers { get; set; }
        public virtual DbSet<CohortEntity> Cohorts { get; set; }
        public virtual DbSet<ComponentEntity> Components { get; set; }
        public virtual DbSet<PollEntity> Polls { get; set; }
        public virtual DbSet<PollInstanceEntity> PollInstances { get; set; }
        public virtual DbSet<PollVariableJoin> PollVariables { get; set; }
        public virtual DbSet<StudentEntity> Students { get; set; }
        public virtual DbSet<StudentCohortJoin> StudentCohorts { get; set; }
        public virtual DbSet<StudentDetailEntity> StudentDetails { get; set; }
        public virtual DbSet<VariableEntity> Variables { get; set; }
        public virtual DbSet<EvaluationEntity> Evaluations { get; set; }
        public virtual DbSet<EvaluationPollJoin> EvaluationPolls { get; set; }

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
