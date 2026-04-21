using Microsoft.Extensions.DependencyInjection;
using Portfolio.Domain.Interface;
using Portfolio.Domain.Services;

namespace Portfolio.Domain
{
    public static class DomainContainer
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDomainService<>), typeof(DomainService<>));
            services.AddScoped<IMensagemService, MensagemService>();

            return services;
        }
    }
}
