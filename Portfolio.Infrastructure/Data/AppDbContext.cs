using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mensagem> Mensagens => Set<Mensagem>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Habilidade> Habilidades => Set<Habilidade>();
        public DbSet<ProjetoCarousel> ProjetosCarousel => Set<ProjetoCarousel>();
        public DbSet<Trabalho> Trabalhos => Set<Trabalho>();
        public DbSet<LinkSocial> LinksSociais => Set<LinkSocial>();
        public DbSet<Contato> Contatos => Set<Contato>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
