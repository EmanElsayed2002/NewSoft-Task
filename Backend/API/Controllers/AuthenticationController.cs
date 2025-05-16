using API.Base;
using Application.abstracts;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] _RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return NewResult(result);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request)
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




    }
}