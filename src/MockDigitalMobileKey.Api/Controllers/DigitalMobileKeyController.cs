using Microsoft.AspNetCore.Mvc;

namespace MockDigitalMobileKey.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DigitalMobileKeyController : ControllerBase
    {
        private readonly Dictionary<string, User> _userValues = [];

        public DigitalMobileKeyController() 
        {
            _userValues.Add("318646883", new User()
            {
                FullName = "João Silva",
                Email = "joao.silva@example.com",
                PhoneNumber = "912345678",
                NIF = "318646883"
            });
        }

        /// <summary>
        /// Add a user in the constructor if need it
        /// </summary>
        /// <param name="nif">NIF of the user</param>
        /// <returns>User</returns>
        [HttpGet("{nif}")]
        public ActionResult<User> GetUserInfoByNIF(string nif)
        {
            if (_userValues.TryGetValue(nif, out User value))
                return Ok(value);

            return NotFound(new { Message = "NIF not found" });
        }
    }
}