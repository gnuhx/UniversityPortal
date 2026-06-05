using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class MonHocConfiguration : IEntityTypeConfiguration<MonHoc>
{
    public void Configure(EntityTypeBuilder<MonHoc> builder)
    {
        builder.ToTable("mon_hoc");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MaMon).IsRequired().HasMaxLength(20).HasColumnName("ma_mon");
        builder.HasIndex(x => x.MaMon).IsUnique();
        builder.Property(x => x.TenMon).IsRequired().HasMaxLength(150).HasColumnName("ten_mon");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
