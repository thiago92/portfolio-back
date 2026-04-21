using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data.Map
{
    public class HabilidadeMap : IEntityTypeConfiguration<Habilidade>
    {
        public void Configure(EntityTypeBuilder<Habilidade> builder)
        {
            builder.ToTable("habilidades");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .ValueGeneratedNever();

            builder.Property(h => h.Label)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.Valor)
                .IsRequired();

            builder.Property(h => h.Ordem)
                .IsRequired();

            builder.HasIndex(h => h.Ordem);
        }
    }
}
