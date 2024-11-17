using Cofidis.Credit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofidis.Credit.Infrastructure.Mappings
{
    public class RiskAnalysisMap : IEntityTypeConfiguration<RiskAnalysis>
    {
        public void Configure(EntityTypeBuilder<RiskAnalysis> builder)
        {
            builder.ToTable("RiskAnalyses");

            builder.HasKey(ra => ra.Id);

            builder.Property(ra => ra.UnemploymentRate)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(ra => ra.InflationRate)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(ra => ra.CreditHistoryScore)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(ra => ra.OutstandingDebts)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(ra => ra.RiskLevel)
                .IsRequired();

            builder.Property(ra => ra.AnalysisDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(ra => ra.User)
                .WithMany(u => u.RiskAnalyses)
                .HasForeignKey(ra => ra.UserId);
        }
    }

}
