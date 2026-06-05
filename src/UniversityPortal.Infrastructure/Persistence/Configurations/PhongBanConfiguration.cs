using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class PhongBanConfiguration : IEntityTypeConfiguration<PhongBan>
{
    public void Configure(EntityTypeBuilder<PhongBan> builder)
    {
        builder.ToTable("phong_ban");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TenPhongBan).IsRequired().HasMaxLength(100).HasColumnName("ten_phong_ban");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
