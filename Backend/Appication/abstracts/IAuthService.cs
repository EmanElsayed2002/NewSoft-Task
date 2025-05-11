using Application.Bases;
using NewSoftTask.Application.DTOs.Authentication;

namespace Application.abstracts
{
    public interface IAuthService
    {
        public Task<Response<AuthResponse>> GetTokenAsync(string email, string password);
        public Task<Response<string>> ConfirmEmailAsync(ConfirmEmailRequest request);
        public Task<Response<string>> RegisterAsync(_RegisterRequest request);
        public Task<Response<string>> ResendConfirmationEmailAsync(string email);
        public Task<Response<string>> ResetPasswordAsync(_ResetPasswordRequest request);
        public Task<Response<string>> ForgotPasswordAsync(string email);

    }
}
