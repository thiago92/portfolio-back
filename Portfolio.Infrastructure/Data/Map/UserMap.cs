using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data.Map
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.CreationTime).IsRequired();
            builder.Property(u => u.CreationUserId).IsRequired();
            builder.Property(u => u.ModificationTime);
            builder.Property(u => u.ModificationUserId);
        }
    }
}
