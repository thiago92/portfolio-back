using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data.Configurations
{
    public class MensagemConfiguration : IEntityTypeConfiguration<Mensagem>
    {
        public void Configure(EntityTypeBuilder<Mensagem> builder)
        {
            builder.ToTable("mensagens");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.Texto)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
