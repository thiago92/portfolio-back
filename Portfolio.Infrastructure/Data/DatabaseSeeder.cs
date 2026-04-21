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

        private static readonly Guid ContatoSingletonId = Guid.Parse("10000000-0000-0000-0000-000000000001");

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

            if (!await context.ProjetosCarousel.AnyAsync(cancellationToken))
            {
                context.ProjetosCarousel.AddRange(
                    new ProjetoCarousel { Url = "https://luanalima.adv.br/direitodasaude/", ImgPath = "/assets/img/carousel/carousel-img-1.png", AltSlug = "altImg1Carousel", Ordem = 1 },
                    new ProjetoCarousel { Url = "https://heniben.vercel.app", ImgPath = "/assets/img/carousel/carousel-img-2.png", AltSlug = "altImg2Carousel", Ordem = 2 },
                    new ProjetoCarousel { Url = "https://isabandeira.com.br", ImgPath = "/assets/img/carousel/carousel-img-3.png", AltSlug = "altImg3Carousel", Ordem = 3 },
                    new ProjetoCarousel { Url = "https://luanalima.adv.br", ImgPath = "/assets/img/carousel/carousel-img-4.png", AltSlug = "altImg4Carousel", Ordem = 4 },
                    new ProjetoCarousel { Url = "https://movimentconsultoria.com", ImgPath = "/assets/img/carousel/carousel-img-5.png", AltSlug = "altImg5Carousel", Ordem = 5 },
                    new ProjetoCarousel { Url = "https://innovatech14.com.br", ImgPath = "/assets/img/carousel/carousel-img-6.png", AltSlug = "altImg6Carousel", Ordem = 6 },
                    new ProjetoCarousel { Url = "https://unicoasfaltosiberia.com.br", ImgPath = "/assets/img/carousel/carousel-img-7.png", AltSlug = "altImg7Carousel", Ordem = 7 },
                    new ProjetoCarousel { Url = "https://goldfishservices.com.br", ImgPath = "/assets/img/carousel/carousel-img-8.png", AltSlug = "altImg8Carousel", Ordem = 8 },
                    new ProjetoCarousel { Url = "https://nascentedavida.com.br", ImgPath = "/assets/img/carousel/carousel-img-9.png", AltSlug = "altImg9Carousel", Ordem = 9 },
                    new ProjetoCarousel { Url = "https://connectsite.com.br", ImgPath = "/assets/img/carousel/carousel-img-10.png", AltSlug = "altImg10Carousel", Ordem = 10 },
                    new ProjetoCarousel { Url = "https://harmonizeclinic.com.br", ImgPath = "/assets/img/carousel/carousel-img-11.png", AltSlug = "altImg11Carousel", Ordem = 11 });
                alterou = true;
            }

            if (!await context.Trabalhos.AnyAsync(cancellationToken))
            {
                context.Trabalhos.AddRange(
                    new Trabalho { TituloSlug = "designMobileEffect", TextoSlug = "textoMobile", ImgPath = "/assets/img/portfolio/portfolio-fourth.jpg", TextoBotaoSlug = "viewProject", Href = "https://thiago92.github.io/sitemadu/", TooltipSlug = "tooltipButtonPortfolio", Ordem = 1 },
                    new Trabalho { TituloSlug = "webDesign", TextoSlug = "textDesign", ImgPath = "/assets/img/portfolio/portfolio-second.jpg", TextoBotaoSlug = "viewProject", Href = "https://thiago92.github.io/stermax/", TooltipSlug = "tooltipButtonPortfolio", Ordem = 2 },
                    new Trabalho { TituloSlug = "responsivoDesign", TextoSlug = "textResponsive", ImgPath = "/assets/img/portfolio/portfolio-third.jpg", TextoBotaoSlug = "viewProject", Href = "https://buzzvel-ebon.vercel.app", TooltipSlug = "tooltipButtonPortfolio", Ordem = 3 },
                    new Trabalho { TituloSlug = "inputEffects", TextoSlug = "textInputEffects", ImgPath = "/assets/img/portfolio/portfolio-fifth.png", TextoBotaoSlug = "viewProject", Href = "https://buzzvel-landingpage-2n83.vercel.app", TooltipSlug = "tooltipButtonPortfolio", Ordem = 4 },
                    new Trabalho { TituloSlug = "ecommerceDesign", TextoSlug = "textEcommerceDesign", ImgPath = "/assets/img/portfolio/portfolio-sixth.png", TextoBotaoSlug = "viewProject", Href = "https://landing-page-e-commerce-two.vercel.app", TooltipSlug = "tooltipButtonPortfolio", Ordem = 5 });
                alterou = true;
            }

            if (!await context.LinksSociais.AnyAsync(cancellationToken))
            {
                context.LinksSociais.AddRange(
                    new LinkSocial { IconeSlug = "instagram", Url = "https://www.instagram.com/thiago.dsouza1992/", Tipo = ETipoLinkSocial.Social, Ordem = 1 },
                    new LinkSocial { IconeSlug = "facebook", Url = "https://web.facebook.com/thiago.portugath", Tipo = ETipoLinkSocial.Social, Ordem = 2 },
                    new LinkSocial { IconeSlug = "linkedin", Url = "https://www.linkedin.com/in/thiago-silva-souza-27557b1a4/", Tipo = ETipoLinkSocial.Social, Ordem = 3 },
                    new LinkSocial { IconeSlug = "whatsapp", Url = "https://wa.me/5541999454655", Tipo = ETipoLinkSocial.ContatoRapido, Ordem = 1 },
                    new LinkSocial { IconeSlug = "telegram", Url = "https://t.me/+5541999454655", Tipo = ETipoLinkSocial.ContatoRapido, Ordem = 2 });
                alterou = true;
            }

            if (!await context.Contatos.AnyAsync(cancellationToken))
            {
                context.Contatos.Add(new Contato
                {
                    Id = ContatoSingletonId,
                    Nome = "Thiago da Silva de Souza",
                    Localizacao = "Paraná - BR",
                    Telefone = "+55(041)99945-4655",
                    Email = "thiago.dsouza1992@gmail.com"
                });
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
