using Application.DTOs.User;
using FluentValidation;

namespace Application.DTOs.UserValidation
{
    public class UserValidation : AbstractValidator<RegisterRequestDTO>
    {
        public UserValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please provide a valid email address");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MinimumLength(3)
                .WithMessage("Full name must be at least 3 characters long");

            RuleFor(x => x.Address)
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("Address must be at least 5 characters long");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmedPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\+?[0-9]{8,15}$")
                .WithMessage("Please provide a valid phone number");

            RuleFor(x => x.Age)
                .GreaterThan(10)
                .WithMessage("Age must be greater than 10");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "Student", "Instructor" }.Contains(role))
                .WithMessage("Role must be either 'Student' or 'Instructor'");
        }
    }
}