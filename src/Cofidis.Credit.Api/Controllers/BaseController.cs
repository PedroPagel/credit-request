using Cofidis.Credit.Domain.Services.Notificator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cofidis.Credit.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public abstract class BaseController(INotificator notificator) : ControllerBase
    {
        private readonly INotificator _notificator = notificator;

        [NonAction]
        protected async Task<ActionResult> CustomResponse(ModelStateDictionary modelState)
        {
            return await Task.FromResult(BadRequest(new
            {
                success = false,
                errors = modelState.Values.SelectMany(v => v.Errors).ToArray()
            }));
        }

        [NonAction]
        public async Task<ActionResult> CustomResponse(object result)
        {
            if (!_notificator.GetErrorNotifications().Any())
            {
                return await Task.FromResult(new OkObjectResult(new
                {
                    success = true,
                    data = result,
                }));
            }

            throw new Exception(string.Join(Environment.NewLine, _notificator.GetErrorNotifications().Select(n => n.Message)));
        }
    }
}
