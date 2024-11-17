using Cofidis.Credit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofidis.Credit.Infrastructure.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(u => u.Nif)
                .IsRequired()
                .HasMaxLength(9);

            builder.Property(u => u.MonthlyIncome)
                .HasColumnType("decimal(18,2)");

            builder.Property(u => u.RegistrationDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasMany(u => u.CreditRequests)
                .WithOne(cr => cr.User)
                .HasForeignKey(cr => cr.UserId);

            builder.HasMany(u => u.RiskAnalyses)
                .WithOne(ra => ra.User)
                .HasForeignKey(ra => ra.UserId);
        }
    }

}
