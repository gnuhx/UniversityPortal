using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class DanhSachLopHPConfiguration : IEntityTypeConfiguration<DanhSachLopHP>
{
    public void Configure(EntityTypeBuilder<DanhSachLopHP> builder)
    {
        builder.ToTable("danh_sach_lop_hp");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.LopHpId).HasColumnName("lop_hp_id");
        builder.Property(x => x.LoaiDangKy).IsRequired().HasMaxLength(20).HasColumnName("loai_dang_ky");
        builder.Property(x => x.TrangThaiDuyet).IsRequired().HasMaxLength(20).HasColumnName("trang_thai_duyet");
        builder.Property(x => x.NguoiDuyetId).HasColumnName("nguoi_duyet_id");
        builder.Property(x => x.DiemQt1).HasColumnName("diem_qt1");
        builder.Property(x => x.DiemQt2).HasColumnName("diem_qt2");
        builder.Property(x => x.DiemThi).HasColumnName("diem_thi");
        builder.Property(x => x.DiemTongKet).HasColumnName("diem_tong_ket");
        builder.Property(x => x.SoTienPhaiDong).HasColumnType("decimal(12,2)").HasColumnName("so_tien_phai_dong");
        builder.Property(x => x.TrangThaiDongTien).HasMaxLength(20).HasColumnName("trang_thai_dong_tien");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.SinhVienId, x.LopHpId }).IsUnique();
        builder.HasOne(x => x.SinhVien).WithMany(s => s.DanhSachLopHPs).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.LopHocPhan).WithMany(l => l.DanhSachLopHPs).HasForeignKey(x => x.LopHpId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.NguoiDuyet).WithMany().HasForeignKey(x => x.NguoiDuyetId).OnDelete(DeleteBehavior.NoAction);
    }
}
