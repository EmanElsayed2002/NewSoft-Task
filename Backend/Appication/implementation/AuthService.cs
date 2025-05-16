using Application.abstracts;
using Application.Bases;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using NewSoftTask.Application.DTOs.Authentication;
using NewSoftTask.Application.Services.Abstract;
using System.Security.Cryptography;
using System.Text;

namespace Application.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IResponseHandler _responseHandler;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly IEmailSender _emailSender;

        private readonly int _refreshTokenExpiryDays = 14;

        public AuthService(
            UserManager<User> userManager,
            IJwtProvider jwtProvider,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            RoleManager<Role> roleManager,
            IResponseHandler responseHandler)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _responseHandler = responseHandler;
        }

        public async Task<Response<AuthResponse>> GetTokenAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return _responseHandler.UnprocessableEntity<AuthResponse>("Invalid credentials");

            if (!user.EmailConfirmed)
                return _responseHandler.UnprocessableEntity<AuthResponse>("Email not confirmed");

            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
            if (!result.Succeeded)
            {
                return result.IsLockedOut
                    ? _responseHandler.UnprocessableEntity<AuthResponse>("Account locked out")
                    : _responseHandler.UnprocessableEntity<AuthResponse>("Invalid credentials");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.tokens.Add(new Token
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                RefreshTokenExpired = refreshTokenExpiration,
            });

            await _userManager.UpdateAsync(user);

            return _responseHandler.Success(new AuthResponse(
                user.Id.ToString(),
                user.Email,
                user.FullName,
                token,
                expiresIn,
                refreshToken,
                refreshTokenExpiration,
                userRoles.FirstOrDefault()));
        }

        public async Task<Response<string>> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return _responseHandler.NotFound<string>("User not found");

            if (user.EmailConfirmed)
                return _responseHandler.Success<string>("Email already confirmed");

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded)
                {
                    var error = result.Errors.FirstOrDefault();
                    return _responseHandler.UnprocessableEntity<string>(
                        error?.Description ?? "Email confirmation failed");
                }

                await _userManager.AddToRoleAsync(user, "Student");
                return _responseHandler.Success<string>("Email confirmed successfully");
            }
            catch (FormatException)
            {
                return _responseHandler.BadRequest<string>("Invalid token format");
            }
        }

        public async Task<Response<string>> RegisterAsync(_RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return _responseHandler.UnprocessableEntity<string>("Email already exists");

            var user = new User()
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName,
                Age = request.Age,
                Address = request.Address
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return _responseHandler.UnprocessableEntity<string>(result.Errors.First().Description);

            if (!await _roleManager.RoleExistsAsync(request.Role))
                return _responseHandler.BadRequest<string>("Specified role does not exist");

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
                return _responseHandler.UnprocessableEntity<string>(roleResult.Errors.First().Description);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await SendEmailConfirmation(user, code);
            return _responseHandler.Success<string>("Registration successful. Please check your email.");
        }

        public async Task<Response<string>> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return _responseHandler.NotFound<string>("User not found");

            if (user.EmailConfirmed)
                return _responseHandler.Success<string>("Email already confirmed");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await SendEmailConfirmation(user, code);
            return _responseHandler.Success<string>("Confirmation email resent successfully");
        }

        public async Task<Response<string>> ResetPasswordAsync(_ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return _responseHandler.NotFound<string>("User not found");

            if (!user.EmailConfirmed)
                return _responseHandler.UnprocessableEntity<string>("Email not confirmed");

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                var result = await _userManager.ResetPasswordAsync(user, code, request.newPassword);

                return result.Succeeded
                    ? _responseHandler.Success<string>("Password reset successfully")
                    : _responseHandler.UnprocessableEntity<string>(result.Errors.First().Description);
            }
            catch (FormatException)
            {
                return _responseHandler.BadRequest<string>("Invalid token format");
            }
        }

        public async Task<Response<string>> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return _responseHandler.NotFound<string>("User not found");

            if (!user.EmailConfirmed)
                return _responseHandler.UnprocessableEntity<string>("Email not confirmed");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await SendPasswordResetEmail(user, code);
            return _responseHandler.Success<string>("Password reset email sent successfully");
        }

        private static string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


        private async Task SendEmailConfirmation(User user, string code)
        {
            try
            {
                var origin = "http://localhost:4200";
                var confirmationUrl = $"{origin}/confirm-email?userId={user.Id}&code={Uri.EscapeDataString(code)}";

                var message = $"""
                <html>
                <body>
                    <p>Hello {user.FullName},</p>
                    <p>Please confirm your email by clicking the link below:</p>
                    <p><a href="{confirmationUrl}" target="_blank" style="color: #1a73e8;">Confirm Email</a></p>
                    <p>Or copy and paste this URL into your browser:</p>
                    <p>{confirmationUrl}</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>Thanks,<br/>Your App Team</p>
                </body>
                </html>
                """;

                await _emailSender.SendEmail(
                    user.Email, message, "Confirm your email"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
                throw;
            }
        }


        private async Task SendPasswordResetEmail(User user, string code)
        {
            var origin = "http://localhost:4200";
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "PasswordReset.html");

            var body = await File.ReadAllTextAsync(templatePath);
            body = body
                .Replace("[name]", user.FullName)
                .Replace("[action_url]", $"{origin}/reset-password?email={user.Email}&code={Uri.EscapeDataString(code)}");

            await _emailSender.SendEmail(user.Email, "Reset your password", body);
        }
    }
}