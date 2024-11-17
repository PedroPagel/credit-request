namespace Cofidis.Credit.Domain.Models.Credits
{
    public class CreditRequested
    {
        public Guid UserId { get; set; }
        public decimal AmountRequested { get; set; }
        public int TermInMonths { get; set; }
    }
}
