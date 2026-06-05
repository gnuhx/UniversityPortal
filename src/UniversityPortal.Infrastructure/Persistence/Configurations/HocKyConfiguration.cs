using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class HocKyConfiguration : IEntityTypeConfiguration<HocKy>
{
    public void Configure(EntityTypeBuilder<HocKy> builder)
    {
        builder.ToTable("hoc_ky");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TenHocKy).IsRequired().HasMaxLength(100).HasColumnName("ten_hoc_ky");
        builder.Property(x => x.NamHocId).HasColumnName("nam_hoc_id");
        builder.Property(x => x.NgayBatDau).HasColumnName("ngay_bat_dau");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.NamHoc).WithMany(n => n.HocKys).HasForeignKey(x => x.NamHocId).OnDelete(DeleteBehavior.Cascade);
    }
}
