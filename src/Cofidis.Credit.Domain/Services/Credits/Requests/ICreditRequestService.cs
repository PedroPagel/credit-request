using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Models.Credits;

namespace Cofidis.Credit.Domain.Services.Credits.Requests
{
    public interface ICreditRequestService
    {
        Task<CreditRequest> ProcessCreditRequest(CreditRequested request);
        Task<CreditRequest> UpdateCredit(Guid id, CreditRequested request);
        Task<CreditRequest> GetCreditRequestById(Guid id);
        Task<CreditRequest> GetCreditRequestByUserId(Guid id);
        Task<bool> DeleteCreditRequest(Guid id);
    }
}
