using FluentValidation;
using Presentation.model;

namespace Presentation.Validetors
{
    public class AuthenticationRequestBodyValidator: AbstractValidator<AuthenticationRequestBody>
    {
        public AuthenticationRequestBodyValidator()
        {
            RuleFor(auth => auth.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(auth => auth.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");
        }
    }
}
