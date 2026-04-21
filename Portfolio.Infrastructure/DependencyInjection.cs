using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Auth;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMensagemRepository, MensagemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHabilidadeRepository, HabilidadeRepository>();
            services.AddScoped<IProjetoCarouselRepository, ProjetoCarouselRepository>();
            services.AddScoped<ITrabalhoRepository, TrabalhoRepository>();
            services.AddScoped<ILinkSocialRepository, LinkSocialRepository>();
            services.AddScoped<IContatoRepository, ContatoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

            return services;
        }
    }
}
