namespace Cofidis.Credit.Domain.Models.Risks
{
    public class RiskAnalysisRequest
    {
        public Guid UserId { get; set; }
        public decimal UnemploymentRate { get; set; }
        public decimal InflationRate { get; set; }
        public decimal CreditHistoryScore { get; set; }
        public decimal OutstandingDebts { get; set; }
    }
}
