using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<ComponentVariableEntity> ComponentVariables { get; set; }
        public DbSet<PollsEntity> Polls { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<AnswersEntity> Answers { get; set; }



        /*
        public DbSet<Courses> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<Rules> Rules { get; set; }
        public DbSet<RiskVariables> RiskVariables { get; set; }
        public DbSet<RiskPopulation> RiskPopulation { get; set; }
        public DbSet<Report> Reports { get; set; }
        */

    }
}
