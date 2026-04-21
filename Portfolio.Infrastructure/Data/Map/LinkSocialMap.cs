using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data.Map
{
    public class LinkSocialMap : IEntityTypeConfiguration<LinkSocial>
    {
        public void Configure(EntityTypeBuilder<LinkSocial> builder)
        {
            builder.ToTable("links_sociais");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.IconeSlug)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(l => l.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(l => l.Ordem)
                .IsRequired();

            builder.HasIndex(l => new { l.Tipo, l.Ordem });
        }
    }
}
