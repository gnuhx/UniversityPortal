using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class LopSinhHoatConfiguration : IEntityTypeConfiguration<LopSinhHoat>
{
    public void Configure(EntityTypeBuilder<LopSinhHoat> builder)
    {
        builder.ToTable("lop_sinh_hoat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MaLop).IsRequired().HasMaxLength(20).HasColumnName("ma_lop");
        builder.HasIndex(x => x.MaLop).IsUnique();
        builder.Property(x => x.GvcnId).HasColumnName("gvcn_id");
        builder.Property(x => x.ThuKyId).HasColumnName("thu_ky_id");
        builder.Property(x => x.ChuongTrinhDtId).HasColumnName("chuong_trinh_dt_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Gvcn).WithMany(g => g.LopSinhHoatGVCN).HasForeignKey(x => x.GvcnId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ThuKy).WithMany(s => s.LopSinhHoatThuKy).HasForeignKey(x => x.ThuKyId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.ChuongTrinhDT).WithMany(c => c.LopSinhHoats).HasForeignKey(x => x.ChuongTrinhDtId).OnDelete(DeleteBehavior.Restrict);
    }
}
