using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class LopHocPhanConfiguration : IEntityTypeConfiguration<LopHocPhan>
{
    public void Configure(EntityTypeBuilder<LopHocPhan> builder)
    {
        builder.ToTable("lop_hoc_phan");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChiTietCtdtId).HasColumnName("chi_tiet_ctdt_id");
        builder.Property(x => x.HocKyId).HasColumnName("hoc_ky_id");
        builder.Property(x => x.GiaoVienId).HasColumnName("giao_vien_id");
        builder.Property(x => x.MaLopHp).IsRequired().HasMaxLength(30).HasColumnName("ma_lop_hp");
        builder.HasIndex(x => x.MaLopHp).IsUnique();
        builder.Property(x => x.KhoaBangDiem).HasDefaultValue(false).HasColumnName("khoa_bang_diem");
        builder.Property(x => x.TrangThaiKetThuc).HasDefaultValue(false).HasColumnName("trang_thai_ket_thuc");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.ChiTietCTDT).WithMany(c => c.LopHocPhans).HasForeignKey(x => x.ChiTietCtdtId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.HocKy).WithMany(h => h.LopHocPhans).HasForeignKey(x => x.HocKyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GiaoVien).WithMany(g => g.LopHocPhans).HasForeignKey(x => x.GiaoVienId).OnDelete(DeleteBehavior.Restrict);
    }
}
