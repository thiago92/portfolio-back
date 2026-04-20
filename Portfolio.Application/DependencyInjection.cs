using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Behaviors;
using Portfolio.Application.Interface;
using Portfolio.Application.Services;

namespace Portfolio.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddScoped(typeof(IAppService<>), typeof(AppService<>));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(assembly);
            services.AddSingleton(mapsterConfig);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
