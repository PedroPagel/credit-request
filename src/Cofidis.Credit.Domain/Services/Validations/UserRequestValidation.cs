using Cofidis.Credit.Domain.Models.Users;
using FluentValidation;

namespace Cofidis.Credit.Domain.Services.Validations
{
    public class UserRequestValidation : AbstractValidator<UserRequest>
    {
        public UserRequestValidation()
        {
            RuleFor(c => c.FullName)
                .NotEmpty()
                .WithMessage("Name cannot be empty");

            RuleFor(c => c.Email)
                .NotEmpty()
                .WithMessage("Email cannot be empty");

            RuleFor(c => c.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number cannot be empty");

            RuleFor(c => c.Nif)
                .NotEmpty()
                .WithMessage("Nif cannot be empty");
        }
    }
}
