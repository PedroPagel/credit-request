using Cofidis.Credit.Domain.Enums;

namespace Cofidis.Credit.Domain.Entities
{
    public class RiskAnalysis : Entity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal UnemploymentRate { get; set; }
        public decimal InflationRate { get; set; }
        public decimal CreditHistoryScore { get; set; }
        public decimal OutstandingDebts { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public DateTime AnalysisDate { get; set; }
        public User User { get; set; }
    }
}
