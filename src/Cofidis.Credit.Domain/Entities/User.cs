namespace Cofidis.Credit.Domain.Entities
{
    public class User : Entity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Nif { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<CreditRequest> CreditRequests { get; set; }
        public ICollection<RiskAnalysis> RiskAnalyses { get; set; }
    }
}
