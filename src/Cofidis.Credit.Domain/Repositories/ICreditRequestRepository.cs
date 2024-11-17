using Cofidis.Credit.Domain.Entities;

namespace Cofidis.Credit.Domain.Repositories
{
    public interface ICreditRequestRepository : IRepository<CreditRequest>
    {
        Task<int> GetCreditLimitByIncome(decimal monthlyIncome);
    }
}
