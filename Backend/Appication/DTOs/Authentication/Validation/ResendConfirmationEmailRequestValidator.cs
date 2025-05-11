using FluentValidation;

namespace NewSoftTask.Application.DTOs.Authentication;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<_ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
