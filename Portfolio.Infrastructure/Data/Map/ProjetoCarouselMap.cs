using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data.Map
{
    public class ProjetoCarouselMap : IEntityTypeConfiguration<ProjetoCarousel>
    {
        public void Configure(EntityTypeBuilder<ProjetoCarousel> builder)
        {
            builder.ToTable("projetos_carousel");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.ImgPath)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.AltSlug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Ordem)
                .IsRequired();

            builder.HasIndex(p => p.Ordem);
        }
    }
}
