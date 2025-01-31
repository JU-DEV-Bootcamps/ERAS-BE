using Eras.Domain.Entities;
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
        public DbSet<Poll> Polls { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<AnswersEntity> Answers { get; set; }
    }
}
