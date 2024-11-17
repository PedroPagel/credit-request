using Cofidis.Credit.Domain.Services.Notificator;
using FluentValidation;
using FluentValidation.Results;

namespace Cofidis.Credit.Domain.Services
{
    public abstract class BaseService(INotificator notificator)
    {
        private readonly INotificator _notificator = notificator;

        protected void NotifyError(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
                NotifyError(error.ErrorMessage);
        }

        protected void NotifyError(string message)
        {
            _notificator.HandleError(new Notification(message));
        }

        protected bool Validate<TV, T>(TV validation, T entry) where TV : AbstractValidator<T>
        {
            var validator = validation.Validate(entry);

            if (validator.IsValid)
                return true;

            NotifyError(validator);

            return false;
        }
    }
}
