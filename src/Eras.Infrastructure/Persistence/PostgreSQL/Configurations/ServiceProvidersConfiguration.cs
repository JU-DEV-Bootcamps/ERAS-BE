using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class ServiceProvidersConfiguration : IEntityTypeConfiguration<ServiceProvidersEntity>
{
    public void Configure(EntityTypeBuilder<ServiceProvidersEntity> Builder)
    {
        Builder.ToTable("serviceProviders");

        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);

        Builder.OwnsOne(Entity => Entity.Audit, Audit =>
        {
            Audit.Property(A => A.CreatedBy)
                .HasColumnName("created_by")
                .HasMaxLength(50)
                .IsRequired();

            Audit.Property(A => A.ModifiedBy)
                .HasColumnName("modified_by")
                .HasMaxLength(50)
                .IsRequired(false);

            Audit.Property(A => A.CreatedAt)
                .HasColumnName("created_at")
                .HasConversion(
                    ValueToInsert => ValueToInsert.ToUniversalTime(),
                    ValueToReturn => DateTime.SpecifyKind(
                        ValueToReturn,
                        DateTimeKind.Utc
                    )
                )
                .IsRequired();

            Audit.Property(A => A.ModifiedAt)
                .HasColumnName("updated_at")
                .HasConversion(
                    ValueToInsert => ValueToInsert.HasValue
                        ? ValueToInsert.Value.ToUniversalTime()
                        : (DateTime?)null,
                    ValueToReturn => ValueToReturn.HasValue
                        ? DateTime.SpecifyKind(
                            ValueToReturn.Value,
                            DateTimeKind.Utc
                        )
                        : (DateTime?)null
                )
                .IsRequired(false);

            Audit.HasData(new 
            {
                ServiceProvidersEntityId = 1,
                CreatedBy = "System",
                ModifiedBy = "System",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = (DateTime?)null

            });
        });

        Builder.HasData(
            new
            {
                Id = 1,
                ServiceProviderName = "Cosmic Latte",
                ServiceProviderLogo = "https://i.imgur.com/cDQU1M7.png"               
            }
        );
    }

    private static void ConfigureColumns(EntityTypeBuilder<ServiceProvidersEntity> Builder)
    {
        Builder.HasKey(Student => Student.Id);
        Builder.Property(Sp => Sp.ServiceProviderName)
            .IsRequired()
            .HasMaxLength(255);
        Builder.Property(Sp => Sp.ServiceProviderLogo)
            .IsRequired();
    }

    private static void ConfigureRelationShips(EntityTypeBuilder<ServiceProvidersEntity> Builder)
    {
        Builder.HasMany(Sp => Sp.Configurations)
            .WithOne(C => C.ServiceProvider)
            .HasForeignKey(C => C.ServiceProviderId);
    }
}
