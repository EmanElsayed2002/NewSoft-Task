﻿using FluentValidation;

namespace NewSoftTask.Application.DTOs.Authentication;

public class RefreshTakenRequestValidator : AbstractValidator<RefreshTakenRequest>
{
    public RefreshTakenRequestValidator()
    {
        RuleFor(x => x.Taken).NotEmpty();
        RuleFor(x => x.RefreshTaken).NotEmpty();
    }
}
