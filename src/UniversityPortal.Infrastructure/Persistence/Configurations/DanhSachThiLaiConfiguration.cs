using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class DanhSachThiLaiConfiguration : IEntityTypeConfiguration<DanhSachThiLai>
{
    public void Configure(EntityTypeBuilder<DanhSachThiLai> builder)
    {
        builder.ToTable("danh_sach_thi_lai");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.LopHpId).HasColumnName("lop_hp_id");
        builder.Property(x => x.DiemThiLai).HasColumnName("diem_thi_lai");
        builder.Property(x => x.SoTienPhaiDong).HasColumnType("decimal(12,2)").HasColumnName("so_tien_phai_dong");
        builder.Property(x => x.TrangThaiDongTien).IsRequired().HasMaxLength(20).HasColumnName("trang_thai_dong_tien");
        builder.Property(x => x.NguoiDuyetId).HasColumnName("nguoi_duyet_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.SinhVien).WithMany(s => s.DanhSachThiLais).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.LopHocPhan).WithMany(l => l.DanhSachThiLais).HasForeignKey(x => x.LopHpId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.NguoiDuyet).WithMany().HasForeignKey(x => x.NguoiDuyetId).OnDelete(DeleteBehavior.NoAction);
    }
}
