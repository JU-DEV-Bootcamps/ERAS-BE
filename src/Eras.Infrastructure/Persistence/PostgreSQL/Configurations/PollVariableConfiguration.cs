using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollVariableConfiguration : IEntityTypeConfiguration<PollVariableJoin>
    {
        public void Configure(EntityTypeBuilder<PollVariableJoin> Builder)
        {
            Builder.ToTable("poll_variable");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollVariableJoin> Builder)
        {
            Builder.HasOne(PollVariable => PollVariable.Poll)
                .WithMany(Poll => Poll.PollVariables)
                .HasForeignKey(PollVariable => PollVariable.PollId)
                .OnDelete(DeleteBehavior.Cascade);
            Builder.HasOne(PollVariable => PollVariable.Variable)
                .WithMany(Variable => Variable.PollVariables)
                .HasForeignKey(PollVariable => PollVariable.VariableId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollVariableJoin> Builder)
        {
            Builder.HasKey(PollVariable => PollVariable.Id);
            Builder.Property(PollVariable => PollVariable.PollId)
                .HasColumnName("poll_id")
                .IsRequired();
            Builder.Property(PollVariable => PollVariable.VariableId)
                .HasColumnName("variable_id")
                .IsRequired();
        }
    }
}
