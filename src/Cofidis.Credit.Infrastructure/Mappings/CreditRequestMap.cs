using Cofidis.Credit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofidis.Credit.Infrastructure.Mappings
{
    public class CreditRequestMap : IEntityTypeConfiguration<CreditRequest>
    {
        public void Configure(EntityTypeBuilder<CreditRequest> builder)
        {
            builder.ToTable("CreditRequests");

            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.AmountRequested)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(cr => cr.ApprovedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(cr => cr.TermInMonths)
                .IsRequired();

            builder.Property(cr => cr.RequestDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(cr => cr.User)
                .WithMany(u => u.CreditRequests)
                .HasForeignKey(cr => cr.UserId);

            builder.HasOne(cr => cr.RiskAnalysis)
                .WithOne()
                .HasForeignKey<CreditRequest>(cr => cr.RiskAnalysisId);
        }
    }

}
