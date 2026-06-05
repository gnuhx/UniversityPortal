using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class GiaoVienConfiguration : IEntityTypeConfiguration<GiaoVien>
{
    public void Configure(EntityTypeBuilder<GiaoVien> builder)
    {
        builder.ToTable("giao_vien");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaiKhoanId).HasColumnName("tai_khoan_id");
        builder.Property(x => x.MaGv).IsRequired().HasMaxLength(20).HasColumnName("ma_gv");
        builder.HasIndex(x => x.MaGv).IsUnique();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.TaiKhoan).WithOne(t => t.GiaoVien).HasForeignKey<GiaoVien>(x => x.TaiKhoanId).OnDelete(DeleteBehavior.Cascade);
    }
}
