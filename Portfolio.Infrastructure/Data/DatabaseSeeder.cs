using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.Interface;

namespace Portfolio.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private const string AdminEmail = "thiago.dsouza1992@gmail.com";
        private const string AdminSenhaInicial = "Admin@2026";

        private static readonly Guid VisitanteUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private const string VisitanteEmail = "visitante@visitante.com.br";
        private const string VisitanteSenhaInicial = "123456";

        public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await EnsureSchemaUpToDateAsync(context, cancellationToken);

            var alterou = false;

            if (!await context.Users.AnyAsync(u => u.Id == AdminUserId, cancellationToken))
            {
                context.Users.Add(new User
                {
                    Id = AdminUserId,
                    Email = AdminEmail,
                    PasswordHash = hasher.Hash(AdminSenhaInicial),
                    Role = ERoleUsuario.Admin,
                    CreationTime = DateTime.UtcNow,
                    CreationUserId = Guid.Empty
                });
                alterou = true;
            }

            if (!await context.Users.AnyAsync(u => u.Id == VisitanteUserId, cancellationToken))
            {
                context.Users.Add(new User
                {
                    Id = VisitanteUserId,
                    Email = VisitanteEmail,
                    PasswordHash = hasher.Hash(VisitanteSenhaInicial),
                    Role = ERoleUsuario.Visitante,
                    CreationTime = DateTime.UtcNow,
                    CreationUserId = Guid.Empty
                });
                alterou = true;
            }

            if (!await context.Habilidades.AnyAsync(cancellationToken))
            {
                context.Habilidades.AddRange(
                    new Habilidade { Label = "HTML/CSS", Valor = 95, Ordem = 1 },
                    new Habilidade { Label = "TypeScript", Valor = 90, Ordem = 2 },
                    new Habilidade { Label = "JavaScript", Valor = 90, Ordem = 3 },
                    new Habilidade { Label = "Angular", Valor = 80, Ordem = 4 },
                    new Habilidade { Label = "React", Valor = 85, Ordem = 5 },
                    new Habilidade { Label = "C#/.NET", Valor = 40, Ordem = 6 },
                    new Habilidade { Label = "Python", Valor = 35, Ordem = 7 },
                    new Habilidade { Label = "SQL", Valor = 50, Ordem = 8 });
                alterou = true;
            }

            if (alterou)
                await context.SaveChangesAsync(cancellationToken);
        }

        private static async Task EnsureSchemaUpToDateAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            var pendentes = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pendentes.Count == 0) return;

            throw new InvalidOperationException(
                $"Existem {pendentes.Count} migration(s) pendente(s): {string.Join(", ", pendentes)}. " +
                "Rode 'dotnet ef database update -p Portfolio.Infrastructure -s PortfolioApi' antes de iniciar a aplicação.");
        }
    }
}
