﻿using FluentValidation;

namespace NewSoftTask.Application.DTOs.Authentication;
public class RegisterRequestValidator : AbstractValidator<_RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty().Length(3, 100);//default in Asp.NetUser is 100

        //RuleFor(x => x.Password)
        //    .Matches(RegexPattern.StrongPassword)
        //    .WithMessage("Password must contains atleast 8 digits, one Uppercase,one Lowercase and NunAlphanumeric");
    }

}
