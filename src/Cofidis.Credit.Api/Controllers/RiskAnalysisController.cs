using AutoMapper;
using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Models.Risks;
using Cofidis.Credit.Domain.Services.Notificator;
using Cofidis.Credit.Domain.Services.Risks.Analysis;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Cofidis.Credit.Api.Controllers
{
    [Route("api/risk-analisys")]
    public class RiskAnalysisController(IRiskAnalysisService riskAnalysisService, IMapper mapper, INotificator notificator) : BaseController(notificator)
    {
        private readonly IMapper _mapper = mapper;
        private readonly IRiskAnalysisService _riskAnalysisService = riskAnalysisService;

        /// <summary>
        /// Add a new risk analisys
        /// </summary>
        /// <param name="request">RiskAnalysisRequest</param>
        /// <returns>RiskAnalysisDto</returns>
        [HttpPost("add-risk")]
        public async Task<ActionResult<RiskAnalysisDto>> AddRiskAnalisys([FromBody] RiskAnalysisRequest request)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<RiskAnalysisDto>(await _riskAnalysisService.AddRiskAnalysis(request));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Update a risk analisys
        /// </summary>
        /// <param name="id">Risk analisys id</param>
        /// <param name="request">RiskAnalysisRequest</param>
        /// <returns>RiskAnalysisDto</returns>
        [HttpPatch("update-risk/{id}")]
        public async Task<ActionResult<RiskAnalysisDto>> UpdateRiskAnalisys(Guid id, [FromBody] RiskAnalysisRequest request)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<RiskAnalysisDto>(await _riskAnalysisService.UpdateRiskAnalysis(id, request));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Get an risk analisys
        /// </summary>
        /// <param name="id">Risk analisys id</param>
        /// <returns>RiskAnalysisDto</returns>
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<RiskAnalysisDto>> GetRiskAnalisysById(Guid id)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<RiskAnalysisDto>(await _riskAnalysisService.GetRiskAnalisysById(id));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Delete a risk analisys
        /// </summary>
        /// <param name="id">Risk analisys id</param>
        /// <returns>True if the risk analisys is removed, false for failure</returns>
        [HttpDelete("delete-risk-analisys/{id}")]
        public async Task<ActionResult<bool>> DeleteRiskAnalisys(Guid id)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = await _riskAnalysisService.DeleteRiskAnalisys(id);

            return await CustomResponse(result);
        }
    }
}
