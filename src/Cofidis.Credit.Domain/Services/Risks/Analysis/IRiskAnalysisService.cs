using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Models.Risks;

namespace Cofidis.Credit.Domain.Services.Risks.Analysis
{
    public interface IRiskAnalysisService
    {
        Task<RiskAnalysis> AddRiskAnalysis(RiskAnalysisRequest request);
        Task<RiskAnalysis> UpdateRiskAnalysis(Guid id, RiskAnalysisRequest request);
        Task<bool> DeleteRiskAnalisys(Guid id);
        Task<RiskAnalysis> GetRiskAnalisysById(Guid id);
    }
}
