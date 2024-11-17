using AutoMapper;
using Cofidis.Credit.Api.Dto;
using Cofidis.Credit.Domain.Models.Users;
using Cofidis.Credit.Domain.Services.Notificator;
using Cofidis.Credit.Domain.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Cofidis.Credit.Api.Controllers
{
    [Route("api/user")]
    public class UserController(IUserService userService, IMapper mapper, INotificator notificator) : BaseController(notificator)
    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="request">UserRequest</param>
        /// <returns>UserDto</returns>
        [HttpPost("add-user")]
        public async Task<ActionResult<UserDto>> AddUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<UserDto>(await _userService.AddUser(request));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Add a new user by the DigitalMobileKey API
        /// </summary>
        /// <param name="nif">The Nif of the user</param>
        /// <param name="monthlyIncome">Monthky income of the user</param>
        /// <returns>UserDto</returns>
        [HttpPost("add-by-digitalkey/{nif}")]
        public async Task<ActionResult<UserDto>> AddUserByDigitalKey(string nif, decimal monthlyIncome)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<UserDto>(await _userService.AddUserByDigitalKey(nif, monthlyIncome));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Update a user 
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="request">UserRequest</param>
        /// <returns>UserDto</returns>
        [HttpPut("update-user/{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = _mapper.Map<UserDto>(await _userService.UpdateUser(id, request));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>UserDto</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var result = _mapper.Map<UserDto>(await _userService.GetUserById(id));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Get a user by the NIF
        /// </summary>
        /// <param name="nif">The Nif of the user</param>
        /// <returns>UserDto</returns>
        [HttpGet("nif/{nif}")]
        public async Task<ActionResult<UserDto>> GetUserByNif(string nif)
        {
            var result = _mapper.Map<UserDto>(await _userService.GetUserByNif(nif));

            return await CustomResponse(result);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>True if the user is removed, false for failure</returns>
        [HttpDelete("delete-user/{id}")]
        public async Task<ActionResult<bool>> DeleteUser(Guid id)
        {
            if (!ModelState.IsValid)
                return await CustomResponse(ModelState);

            var result = await _userService.DeleteUser(id);

            return await CustomResponse(result);
        }
    }
}
