using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollVariableConfiguration : IEntityTypeConfiguration<PollVariableJoin>
    {
        public void Configure(EntityTypeBuilder<PollVariableJoin> builder)
        {
            builder.ToTable("poll_variable");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollVariableJoin> builder)
        {
            builder.HasOne(pollVariable => pollVariable.Poll)
                .WithMany(poll => poll.PollVariables)
                .HasForeignKey(pollVariable => pollVariable.PollId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pollVariable => pollVariable.Variable)
                .WithMany(variable => variable.PollVariables)
                .HasForeignKey(pollVariable => pollVariable.VariableId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollVariableJoin> builder)
        {
            builder.HasKey(pollVariable => pollVariable.Id);
            builder.Property(pollVariable => pollVariable.PollId)
                .HasColumnName("poll_id")
                .IsRequired();
            builder.Property(pollVariable => pollVariable.VariableId)
                .HasColumnName("variable_id")
                .IsRequired();
        }
    }
}