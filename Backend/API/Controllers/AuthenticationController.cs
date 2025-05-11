using API.Base;
using Application.abstracts;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using NewSoftTask.Application.DTOs.Authentication;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ResultController
    {
        private readonly IAuthService _authService;
        private readonly IMemoryCache _cache;
        private readonly UserManager<User> _userManager;

        public AuthenticationController(
            IAuthService authService,
            IMemoryCache memoryCache,
            UserManager<User> userManager)
        {
            _authService = authService;
            _cache = memoryCache;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] _LoginRequest loginRequest)
        {
            var result = await _authService.GetTokenAsync(loginRequest.email, loginRequest.password);
            return NewResult(result);
        }

        [DisableRateLimiting]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] _RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return NewResult(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request);
            return NewResult(result);
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmailAsync(
            [FromBody] _ResendConfirmationEmailRequest request)
        {
            var result = await _authService.ResendConfirmationEmailAsync(request.Email);
            return NewResult(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);
            return NewResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] _ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            return NewResult(result);
        }




    }
}