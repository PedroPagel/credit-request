using Cofidis.Credit.Domain.Enums;

namespace Cofidis.Credit.Api.Dto
{
    public class RiskAnalysisDto
    {
        public Guid UserId { get; set; }
        public decimal UnemploymentRate { get; set; }
        public decimal InflationRate { get; set; }
        public decimal CreditHistoryScore { get; set; }
        public decimal OutstandingDebts { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public DateTime AnalysisDate { get; set; }
    }
}
