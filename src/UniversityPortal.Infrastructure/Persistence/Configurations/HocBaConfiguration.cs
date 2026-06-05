using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class HocBaConfiguration : IEntityTypeConfiguration<HocBa>
{
    public void Configure(EntityTypeBuilder<HocBa> builder)
    {
        builder.ToTable("hoc_ba");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.CtdtId).HasColumnName("ctdt_id");
        builder.Property(x => x.DiemTbcTichLuy).HasColumnName("diem_tbc_tich_luy");
        builder.Property(x => x.SoTinChiTichLuy).HasColumnName("so_tin_chi_tich_luy");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.SinhVienId, x.CtdtId }).IsUnique();
        builder.HasOne(x => x.SinhVien).WithMany(s => s.HocBas).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ChuongTrinhDT).WithMany(c => c.HocBas).HasForeignKey(x => x.CtdtId).OnDelete(DeleteBehavior.Restrict);
    }
}
