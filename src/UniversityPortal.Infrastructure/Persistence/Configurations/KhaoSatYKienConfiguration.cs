using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class KhaoSatYKienConfiguration : IEntityTypeConfiguration<KhaoSatYKien>
{
    public void Configure(EntityTypeBuilder<KhaoSatYKien> builder)
    {
        builder.ToTable("khao_sat_y_kien");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.LopHpId).HasColumnName("lop_hp_id");
        builder.Property(x => x.DiemDanhGia).HasColumnName("diem_danh_gia");
        builder.Property(x => x.GopY).HasColumnType("text").HasColumnName("gop_y");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.SinhVienId, x.LopHpId }).IsUnique();
        builder.HasOne(x => x.SinhVien).WithMany(s => s.KhaoSatYKiens).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.LopHocPhan).WithMany(l => l.KhaoSatYKiens).HasForeignKey(x => x.LopHpId).OnDelete(DeleteBehavior.Cascade);
    }
}
