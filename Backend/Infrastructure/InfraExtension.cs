using Infrastructure.Repos.abstracts;
using Infrastructure.Repos.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfraExtension
    {
        public static IServiceCollection AddInfraExtension(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ISubjectRepo, SubjectRepo>();
            return services;
        }
    }
}
