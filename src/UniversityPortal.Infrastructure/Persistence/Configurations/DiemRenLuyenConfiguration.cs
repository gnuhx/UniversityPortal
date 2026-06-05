using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class DiemRenLuyenConfiguration : IEntityTypeConfiguration<DiemRenLuyen>
{
    public void Configure(EntityTypeBuilder<DiemRenLuyen> builder)
    {
        builder.ToTable("diem_ren_luyen");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.HocKyId).HasColumnName("hoc_ky_id");
        builder.Property(x => x.DiemTong).HasColumnName("diem_tong");
        builder.Property(x => x.XepLoai).IsRequired().HasMaxLength(30).HasColumnName("xep_loai");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.SinhVienId, x.HocKyId }).IsUnique();
        builder.HasOne(x => x.SinhVien).WithMany(s => s.DiemRenLuyens).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.HocKy).WithMany(h => h.DiemRenLuyens).HasForeignKey(x => x.HocKyId).OnDelete(DeleteBehavior.Restrict);
    }
}
