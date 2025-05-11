using Application.abstracts;
using Application.Bases;
using Application.Implementation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NewSoftTask.Application.Services.Abstract;
using NewSoftTask.Application.Services.Implementation;
using System.Reflection;

namespace Application
{
    public static class AppExtension
    {
        public static IServiceCollection addAppExtension(this IServiceCollection services)
        {
            services.AddScoped<IApplicationUserService, ApplicationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<IResponseHandler, ResponseHandler>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IApplicationUserService, ApplicationService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
