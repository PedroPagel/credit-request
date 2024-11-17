namespace Cofidis.Credit.Domain.Entities
{
    public class CreditRequest : Entity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RiskAnalysisId { get; set; }
        public decimal AmountRequested { get; set; }
        public int TermInMonths { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime RequestDate { get; set; }
        public User User { get; set; }
        public RiskAnalysis RiskAnalysis { get; set; }
    }
}
