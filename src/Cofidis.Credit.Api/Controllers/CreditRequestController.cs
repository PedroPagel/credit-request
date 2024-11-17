using AutoMapper;
using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Models.Credits;
using Cofidis.Credit.Domain.Services.Credits.Requests;
using Cofidis.Credit.Domain.Services.Notificator;
using Microsoft.AspNetCore.Mvc;

namespace Cofidis.Credit.Api.Controllers
{
    [ApiController]
    [Route("api/credit-request")]
    public class CreditRequestController(ICreditRequestService creditRequestService, IMapper mapper, INotificator notificator) : BaseController(notificator)
    {
        private readonly ICreditRequestService _creditRequestService = creditRequestService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Add a new credit request
        /// </summary>
        /// <param name="request">CreditRequested</param>
        /// <returns>CreditRequestDto</returns>
        [HttpPost("add-credit")]
        public async Task<ActionResult<CreditRequestDto>> ProcessCreditRequest([FromBody] CreditRequested request)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<CreditRequestDto>(await _creditRequestService.ProcessCreditRequest(request));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Update a credit request
        /// </summary>
        /// <param name="id">Credit request id</param>
        /// <param name="request">CreditRequested</param>
        /// <returns>CreditRequestDto</returns>
        [HttpPatch("update-credit/{id}")]
        public async Task<ActionResult<CreditRequestDto>> UpdateCreditRequest(Guid id, [FromBody] CreditRequested request)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<CreditRequestDto>(await _creditRequestService.UpdateCredit(id, request));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Delete a credit request
        /// </summary>
        /// <param name="id">credit request id</param>
        /// <returns>True if the credit request is removed, false for failure</returns>
        [HttpDelete("delete-credit/{id}")]
        public async Task<ActionResult<bool>> DeleteCreditRequest(Guid id)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = await _creditRequestService.DeleteCreditRequest(id);

            return await CustomResponse(result);
        }

        /// <summary>
        /// Get a credit request by id
        /// </summary>
        /// <param name="id">Credit request id</param>
        /// <returns>CreditRequestDto</returns>
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<CreditRequestDto>> GetCreditRequestById(Guid id)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<CreditRequestDto>(await _creditRequestService.GetCreditRequestById(id));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Get a credit request by userId
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>CreditRequestDto</returns>
        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<CreditRequestDto>> GetCreditRequestsByUserId(Guid userId)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<CreditRequestDto>(await _creditRequestService.GetCreditRequestByUserId(userId));

            return await CustomResponse(result);
        }
    }
}
