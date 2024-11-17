using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Enums;
using Cofidis.Credit.Domain.Models.Risks;
using Cofidis.Credit.Domain.Options;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.Notificator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cofidis.Credit.Domain.Services.Risks.Analysis
{
    public class RiskAnalysisService(INotificator notificator,
                                     ILogger<RiskAnalysisService> logger,
                                     IOptions<RiskAnalysisOptions> options, 
                                     IRiskAnalysisRepository riskAnalysisRepository, 
                                     ICreditRequestService creditRequestService) :
        BaseService(notificator), IRiskAnalysisService
    {
        private readonly ILogger<RiskAnalysisService> _logger = logger;   
        private readonly RiskAnalysisOptions _riskAnalysisOptions = options.Value;
        private readonly IRiskAnalysisRepository _riskAnalysisRepository = riskAnalysisRepository;
        private readonly ICreditRequestService _creditRequestService = creditRequestService;

        internal RiskLevel GetRiskLevel(RiskAnalysisRequest request)
        {
            _logger.LogInformation("Calculating risk level for user: {UserId}", request.UserId);

            const decimal unemploymentWeight = 0.3m;
            const decimal inflationWeight = 0.2m;
            const decimal creditHistoryWeight = 0.4m;
            const decimal debtWeight = 0.1m;

            var normalizedCreditScore = Math.Min(request.CreditHistoryScore / 100, 1);

            var riskIndex =
                (request.UnemploymentRate * unemploymentWeight) +
                (request.InflationRate * inflationWeight) -
                (normalizedCreditScore * creditHistoryWeight) +
                (request.OutstandingDebts * debtWeight);

            var score = Math.Max(riskIndex, 0);
            _logger.LogInformation("Risk score calculated: {Score} for user: {UserId}", score, request.UserId);

            if (score > _riskAnalysisOptions.HighRiskThreshold)
            {
                _logger.LogInformation("Risk level determined as High for user: {UserId}", request.UserId);
                return RiskLevel.High;
            }

            if (score > _riskAnalysisOptions.MediumRiskThreshold)
            {
                _logger.LogInformation("Risk level determined as Medium for user: {UserId}", request.UserId);
                return RiskLevel.Medium;
            }

            _logger.LogInformation("Risk level determined as Low for user: {UserId}", request.UserId);
            return RiskLevel.Low;
        }

        public async Task<RiskAnalysis> AddRiskAnalysis(RiskAnalysisRequest request)
        {
            _logger.LogInformation("Adding risk analysis for user: {UserId}", request.UserId);

            var riskLevel = GetRiskLevel(request);

            var riskAnalysis = new RiskAnalysis
            {
                UserId = request.UserId,
                UnemploymentRate = request.UnemploymentRate,
                InflationRate = request.InflationRate,
                CreditHistoryScore = request.CreditHistoryScore,
                OutstandingDebts = request.OutstandingDebts,
                RiskLevel = riskLevel,
                AnalysisDate = DateTime.UtcNow
            };

            if (!await _riskAnalysisRepository.Add(riskAnalysis))
            {
                NotifyError("It was not possible to add the risk analisys.");
                return null;
            }

            _logger.LogInformation("Risk analysis successfully added for user: {UserId}", request.UserId);
            return riskAnalysis;
        }

        public async Task<bool> DeleteRiskAnalisys(Guid id)
        {
            _logger.LogInformation("Attempting to delete risk analysis with ID: {Id}", id);

            if (id == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return false;
            }

            var risk = await GetRiskAnalisysById(id);

            if (risk is null)
            {
                NotifyError("The risk analisys was not found with the id.");
                return false;
            }

            var credit = await _creditRequestService.GetCreditRequestByUserId(risk.UserId);

            if (credit is not null)
            {
                NotifyError("The analisys has a credit created, the risk analisys will not be removed.");
                return false;
            }

            var result = await _riskAnalysisRepository.Delete(ra => ra.Id == id);

            if (result == 0)
            {
                NotifyError("It was not possible to remove the risk analisys.");
                return false;
            }

            _logger.LogInformation("Risk analysis with ID: {Id} successfully deleted.", id);
            return true;
        }

        public async Task<RiskAnalysis> UpdateRiskAnalysis(Guid id, RiskAnalysisRequest request)
        {
            _logger.LogInformation("Updating risk analysis with ID: {Id}", id);

            var risk = await _riskAnalysisRepository.GetById(id);

            if (risk is null)
            {
                NotifyError("Risk not found");
                return null;
            }

            var riskLevel = GetRiskLevel(request);

            risk.UnemploymentRate = request.UnemploymentRate;
            risk.InflationRate = request.InflationRate;
            risk.CreditHistoryScore = request.CreditHistoryScore;
            risk.OutstandingDebts = request.OutstandingDebts;
            risk.RiskLevel = riskLevel;
            risk.AnalysisDate = DateTime.UtcNow;

            if (!await _riskAnalysisRepository.Update(risk))
            {
                NotifyError("It was not possible to update the risk analisys.");
                return null;
            }

            _logger.LogInformation("Risk analysis with ID: {Id} successfully updated.", id);
            return risk;
        }

        public async Task<RiskAnalysis> GetRiskAnalisysById(Guid id)
        {
            _logger.LogInformation("Fetching risk analysis by ID: {Id}", id);

            if (id == Guid.Empty)
            {
                NotifyError("Id is invalid!");
                return null;
            }

            var risk = await _riskAnalysisRepository.GetById(id);

            _logger.LogInformation(risk != null ? "Risk analysis found with ID: {Id}" : "No risk analysis found with ID: {Id}", id);
            return risk;
        }
    }
}
