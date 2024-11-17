using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Cofidis.Credit.Infrastructure.Data
{
    public class CreditDbContext(DbContextOptions<CreditDbContext> options) : DbContext(options)
    {
        public DbSet<CreditRequest> CreditRequests { get; set; }
        public DbSet<RiskAnalysis> RiskAnalyses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new CreditRequestMap());
            modelBuilder.ApplyConfiguration(new RiskAnalysisMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
