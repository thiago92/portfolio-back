using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data.Map
{
    public class TrabalhoMap : IEntityTypeConfiguration<Trabalho>
    {
        public void Configure(EntityTypeBuilder<Trabalho> builder)
        {
            builder.ToTable("trabalhos");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.TituloSlug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.TextoSlug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.ImgPath)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(t => t.TextoBotaoSlug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Href)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.TooltipSlug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Ordem)
                .IsRequired();

            builder.HasIndex(t => t.Ordem);
        }
    }
}
