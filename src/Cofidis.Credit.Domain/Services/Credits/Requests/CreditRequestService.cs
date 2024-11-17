using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Enums;
using Cofidis.Credit.Domain.Models.Credits;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Notificator;
using Microsoft.Extensions.Logging;

namespace Cofidis.Credit.Domain.Services.Credits.Requests
{
    public class CreditRequestService(INotificator notificator,
                                      ILogger<CreditRequestService> logger,
                                      IRiskAnalysisRepository riskAnalysisRepository,
                                      ICreditRequestRepository creditRequestRepository,
                                      IUserRepository userRepository) : BaseService(notificator), ICreditRequestService
    {
        private readonly IRiskAnalysisRepository _riskAnalysisRepository = riskAnalysisRepository;
        private readonly ICreditRequestRepository _creditRequestRepository = creditRequestRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<CreditRequestService> _logger = logger;

        private async Task<decimal> GetApprovedAmountByRisk(CreditRequested request, decimal monthlyIncome, RiskLevel riskLevel)
        {
            _logger.LogInformation("Calculating approved amount for UserId: {UserId} with requested amount: {AmountRequested}", request.UserId, request.AmountRequested);
            var creditLimit = await _creditRequestRepository.GetCreditLimitByIncome(monthlyIncome);

            var approvedAmount = riskLevel == RiskLevel.Low
                ? request.AmountRequested
                : request.AmountRequested * 0.8m;

            if (approvedAmount > creditLimit)
            {
                _logger.LogWarning("Approved amount {ApprovedAmount} exceeds credit limit {CreditLimit}. Setting approved amount to credit limit.", approvedAmount, creditLimit);
                approvedAmount = creditLimit;
            }

            return approvedAmount;
        }

        public async Task<bool> DeleteCreditRequest(Guid id)
        {
            if (id == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return false;
            }

            _logger.LogInformation("Deleting credit request with Id: {Id}", id);
            var credit = await GetCreditRequestById(id);

            if (credit is null)
            {
                NotifyError(string.Format("The credit was not foud for the id: {Id}", id));
                return false;
            }

            var result = await _creditRequestRepository.Delete(cr => cr.Id == credit.Id);

            if (result == 0)
            {
                NotifyError("It was not possible to remove the credit requested.");
                return false;
            }

            _logger.LogInformation("Successfully deleted credit request with Id: {Id}", id);
            return true;
        }

        public async Task<CreditRequest> ProcessCreditRequest(CreditRequested request)
        {
            _logger.LogInformation("Processing credit request for UserId: {UserId}", request.UserId);

            if (request.AmountRequested <= 0 || request.TermInMonths <= 0)
            {
                NotifyError("The requested values are incorrect");
                return null;
            }

            var user = await _userRepository.FirstOrDefault(u => u.Id == request.UserId);

            if (user is null)
            {
                NotifyError("User not found.");
                return null;
            }

            var credit = await GetCreditRequestByUserId(user.Id);

            if (credit is not null)
            {
                NotifyError(string.Format("A credit already exists for the user: {user.Id}", user.Id));
                return null;
            }

            var riskAnalysis = await _riskAnalysisRepository.FirstOrDefault(ra => ra.UserId == request.UserId);

            if (riskAnalysis is null)
            {
                NotifyError("Risk analysis is required before processing a credit request.");
                return null;
            }

            if (riskAnalysis.RiskLevel == RiskLevel.High)
            {
                NotifyError("Credit cannot be granted for high-risk analyses.");
                return null;
            }

            var approvedAmount = await GetApprovedAmountByRisk(request, user.MonthlyIncome, riskAnalysis.RiskLevel);

            var creditRequest = new CreditRequest
            {
                UserId = request.UserId,
                RiskAnalysisId = riskAnalysis.Id,
                AmountRequested = request.AmountRequested,
                TermInMonths = request.TermInMonths,
                ApprovedAmount = approvedAmount,
                RequestDate = DateTime.UtcNow
            };

            if (!await _creditRequestRepository.Add(creditRequest))
            {
                NotifyError("It was not possible to add the credit requested.");
                return null;
            }

            _logger.LogInformation("Successfully processed credit request for UserId: {UserId}", request.UserId);

            return creditRequest;
        }

        public async Task<CreditRequest> UpdateCredit(Guid id, CreditRequested request)
        {
            _logger.LogInformation("Updating credit request with Id: {Id}", id);

            if (request.AmountRequested <= 0)
            {
                NotifyError(string.Format("The requested amount of: {request.AmountRequested}, is incorrect", request.AmountRequested));
                return null;
            }

            var credit = await GetCreditRequestById(id);

            if (credit is null)
            {
                NotifyError("Credit not found");
                return null;
            }

            var user = await _userRepository.FirstOrDefault(u => u.Id == request.UserId);

            if (user is null)
            {
                NotifyError("User not found.");
                return null;
            }

            var riskAnalysis = await _riskAnalysisRepository.FirstOrDefault(ra => ra.UserId == request.UserId);

            if (riskAnalysis is null)
            {
                NotifyError("Risk analysis is required before processing a credit request.");
                return null;
            }

            if (riskAnalysis.RiskLevel == RiskLevel.High)
            {
                NotifyError("Credit cannot be granted for high-risk analyses.");
                return null;
            }

            var approvedAmount = await GetApprovedAmountByRisk(request, user.MonthlyIncome, riskAnalysis.RiskLevel);

            credit.RequestDate = DateTime.UtcNow;
            credit.TermInMonths = request.TermInMonths;
            credit.ApprovedAmount = approvedAmount;
            credit.AmountRequested = request.AmountRequested;

            if (!await _creditRequestRepository.Update(credit))
            {
                NotifyError("It was not possible to update the credit requested.");
                return null;
            }

            _logger.LogInformation("Successfully updated credit request with ID: {Id}", id);
            return credit;
        }

        public async Task<CreditRequest> GetCreditRequestById(Guid id)
        {
            if (id == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return null;
            }

            var credit = await _creditRequestRepository.GetById(id);

            return credit;
        }

        public async Task<CreditRequest> GetCreditRequestByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return null;
            }

            var credit = await _creditRequestRepository.FirstOrDefault(cr => cr.UserId == userId);

            return credit;
        }
    }
}
