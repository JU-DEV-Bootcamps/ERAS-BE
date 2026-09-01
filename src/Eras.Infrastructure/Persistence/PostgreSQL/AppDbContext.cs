using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.FeatureFlagManagement;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class AppDbContext : DbContext, IDataBase
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
        public virtual DbSet<ConfigurationsEntity> Configurations { get; set; }
        public virtual DbSet<ServiceProvidersEntity> ServiceProviders { get; set; }
        public virtual DbSet<UserPollsEntity> UserPolls { get; set; }
        public virtual DbSet<JURemissionEntity> JURemissions { get; set; }
        public virtual DbSet<JUInterventionEntity> JUInterventions { get; set; }
        public virtual DbSet<JUProfessionalEntity> Professionals { get; set; }
        public virtual DbSet<JUServiceEntity> JUServices { get; set; }
        public virtual DbSet<ImportJobEntity> ImportJobs { get; set; }
        public virtual DbSet<ImportJobItemEntity> ImportJobItems { get; set; }
        public virtual DbSet<AttachmentDraftSessionEntity> AttachmentDraftSessions { get; set; }
        public DbSet<Assessment> Assessments => Set<Assessment>();
        public DbSet<Intervention> Interventions => Set<Intervention>();
        public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<DataMigrationCompletion> DataMigrationCompletions => Set<DataMigrationCompletion>();

        // Views
        public virtual DbSet<ErasCalculationsByPollEntity> ErasCalculationsByPoll { get; set; }
        public virtual DbSet<ErasEvaluationDetailsViewEntity> ErasEvaluationDetailsView { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> Options)
            : base(Options)
        {
        }

        protected override void OnModelCreating(ModelBuilder ModelBuilder)
        {
            ModelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(ModelBuilder);
        }

        /// <summary>
        /// Brings the database schema up to date and recreates the SQL views that depend on it
        /// </summary>
        public async Task MigrateAsync(CancellationToken CancellationToken = default)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            await RunViewScriptsAsync(Path.Combine(basePath, "Persistence/PostgreSQL/Views/Drops"), CancellationToken);

            await Database.MigrateAsync(CancellationToken);

            await RunViewScriptsAsync(Path.Combine(basePath, "Persistence/PostgreSQL/Views/Ups"), CancellationToken);
        }

        /// <summary>
        /// Runs every `*.sql` file in <paramref name="ScriptsPath"/>, in name order. Goes through
        /// <see cref="Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync"/>
        /// rather than manually opening/disposing <c>Database.GetDbConnection()</c>
        /// </summary>
        private async Task RunViewScriptsAsync(string ScriptsPath, CancellationToken CancellationToken)
        {
            if (!Directory.Exists(ScriptsPath))
                return;

            IOrderedEnumerable<string> files = Directory.GetFiles(ScriptsPath, "*.sql").OrderBy(file => file);

            foreach (string file in files)
            {
                string sql = await File.ReadAllTextAsync(file, CancellationToken);
                await Database.ExecuteSqlRawAsync(sql, CancellationToken);
            }
        }
    }
}
