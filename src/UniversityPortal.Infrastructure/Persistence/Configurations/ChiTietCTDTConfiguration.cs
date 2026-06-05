using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ChiTietCTDTConfiguration : IEntityTypeConfiguration<ChiTietCTDT>
{
    public void Configure(EntityTypeBuilder<ChiTietCTDT> builder)
    {
        builder.ToTable("chi_tiet_ctdt");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CtdtId).HasColumnName("ctdt_id");
        builder.Property(x => x.MonHocId).HasColumnName("mon_hoc_id");
        builder.Property(x => x.HocKyId).HasColumnName("hoc_ky_id");
        builder.Property(x => x.SoTinChi).HasColumnName("so_tin_chi");
        builder.Property(x => x.TinhDiemTb).HasDefaultValue(true).HasColumnName("tinh_diem_tb");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.ChuongTrinhDT).WithMany(c => c.ChiTietCTDTs).HasForeignKey(x => x.CtdtId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.MonHoc).WithMany(m => m.ChiTietCTDTs).HasForeignKey(x => x.MonHocId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.HocKy).WithMany(h => h.ChiTietCTDTs).HasForeignKey(x => x.HocKyId).OnDelete(DeleteBehavior.Restrict);
    }
}
