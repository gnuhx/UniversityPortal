using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class SinhVienConfiguration : IEntityTypeConfiguration<SinhVien>
{
    public void Configure(EntityTypeBuilder<SinhVien> builder)
    {
        builder.ToTable("sinh_vien");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaiKhoanId).HasColumnName("tai_khoan_id");
        builder.Property(x => x.Mssv).IsRequired().HasMaxLength(20).HasColumnName("mssv");
        builder.HasIndex(x => x.Mssv).IsUnique();
        builder.Property(x => x.LopId).HasColumnName("lop_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.TaiKhoan).WithOne(t => t.SinhVien).HasForeignKey<SinhVien>(x => x.TaiKhoanId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Lop).WithMany(l => l.SinhViens).HasForeignKey(x => x.LopId).OnDelete(DeleteBehavior.SetNull);
    }
}
