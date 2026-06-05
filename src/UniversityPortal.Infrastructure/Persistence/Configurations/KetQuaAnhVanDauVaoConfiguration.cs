using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class KetQuaAnhVanDauVaoConfiguration : IEntityTypeConfiguration<KetQuaAnhVanDauVao>
{
    public void Configure(EntityTypeBuilder<KetQuaAnhVanDauVao> builder)
    {
        builder.ToTable("ket_qua_anh_van_dau_vao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.HasIndex(x => x.SinhVienId).IsUnique();
        builder.Property(x => x.HinhThucXet).IsRequired().HasMaxLength(50).HasColumnName("hinh_thuc_xet");
        builder.Property(x => x.DiemThi).HasColumnName("diem_thi");
        builder.Property(x => x.DiemTa1).HasColumnName("diem_ta1");
        builder.Property(x => x.DiemTa2).HasColumnName("diem_ta2");
        builder.Property(x => x.DiemTa3).HasColumnName("diem_ta3");
        builder.Property(x => x.GhiChu).HasMaxLength(255).HasColumnName("ghi_chu");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.SinhVien).WithOne(s => s.KetQuaAnhVanDauVao).HasForeignKey<KetQuaAnhVanDauVao>(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
    }
}
