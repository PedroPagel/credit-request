namespace Cofidis.Credit.Api.Dto
{
    public class CreditRequestDto
    {
        public Guid Id { get; set; }
        public decimal AmountRequested { get; set; }
        public int TermInMonths { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime RequestDate { get; set; }
        public RiskAnalysisDto RiskAnalysis { get; set; }
    }
}
