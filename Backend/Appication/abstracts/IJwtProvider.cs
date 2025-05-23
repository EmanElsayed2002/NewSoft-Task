﻿using Domain.Models;

namespace NewSoftTask.Application.Services.Abstract
{
    public interface IJwtProvider
    {
        public JwtResult GenerateToken(User user, IEnumerable<string> roles);

        string? ValidateTaken(string taken);
    }
    public record JwtResult(string taken, int expireIn);
}
