using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class HocPhiConfiguration : IEntityTypeConfiguration<HocPhi>
{
    public void Configure(EntityTypeBuilder<HocPhi> builder)
    {
        builder.ToTable("hoc_phi");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.HocKyId).HasColumnName("hoc_ky_id");
        builder.Property(x => x.SoTien).HasColumnType("decimal(12,2)").HasColumnName("so_tien");
        builder.Property(x => x.TrangThaiDong).IsRequired().HasMaxLength(20).HasColumnName("trang_thai_dong");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.SinhVienId, x.HocKyId }).IsUnique();
        builder.HasOne(x => x.SinhVien).WithMany(s => s.HocPhis).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.HocKy).WithMany(h => h.HocPhis).HasForeignKey(x => x.HocKyId).OnDelete(DeleteBehavior.Restrict);
    }
}
