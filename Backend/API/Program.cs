using Application;
using Application.Helper;
using Domain.Models;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Seeder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewSoftTask.Application.Settings;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<ValidationExceptionFilter>();
            builder.Services.AddControllers()
           .AddFluentValidation(fv =>
           {
               fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
               fv.ImplicitlyValidateChildProperties = true;
           });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationExceptionFilter>();
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
            });
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
             .AddEntityFrameworkStores<AppDbContext>()
             .AddDefaultTokenProviders();
            builder.Services.AddInfraExtension().addAppExtension();
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddTransient<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            var EmailSetting = new EmailSetting();
            builder.Configuration.GetSection(nameof(EmailSetting)).Bind(EmailSetting);
            builder.Services.AddSingleton(EmailSetting);

            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));

            var settings = builder.Configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
         )
         .AddJwtBearer(o =>
         {
             o.SaveToken = true;
             o.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                 ValidateIssuer = true,
                 ValidIssuer = settings.Issuer,
                 ValidateAudience = true,
                 ValidAudience = settings.Audience,
                 ValidateLifetime = true,
                 ClockSkew = TimeSpan.Zero,
                 NameClaimType = "sub",
                 RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
             };
             o.Events = new JwtBearerEvents
             {
                 OnMessageReceived = context =>
                 {
                     if (!string.IsNullOrEmpty(context.Token))
                     {

                         var cleanToken = new string(context.Token
                             .Where(c => !char.IsWhiteSpace(c) && !char.IsControl(c))
                             .ToArray());

                         Console.WriteLine($"Original token: '{context.Token}'");
                         Console.WriteLine($"Cleaned token: '{cleanToken}'");
                         Console.WriteLine($"Original length: {context.Token.Length}");
                         Console.WriteLine($"Cleaned length: {cleanToken.Length}");

                         context.Token = cleanToken;
                     }
                     return Task.CompletedTask;
                 },
                 OnAuthenticationFailed = context =>
                 {
                     Console.WriteLine($"Auth failed: {context.Exception}");
                     return Task.CompletedTask;
                 }
             };
         });
            builder.Services.(); AddAuthorization
            builder.Services.AddRateLimiter(RateLimiterOption =>
            {
                RateLimiterOption.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                RateLimiterOption.AddTokenBucketLimiter("Token", option =>
                {
                    option.TokenLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    option.TokensPerPeriod = 2;
                    option.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                    option.AutoReplenishment = true;
                }
                );
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Courses", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            builder.Services.AddMemoryCache();
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                await RoleSeeder.SeedAsync(roleManager);
                await UserSeeder.SeedAsync(userManager);
            }


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("Courses");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
