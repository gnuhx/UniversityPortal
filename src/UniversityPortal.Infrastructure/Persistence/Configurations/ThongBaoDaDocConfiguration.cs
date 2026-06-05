using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ThongBaoDaDocConfiguration : IEntityTypeConfiguration<ThongBaoDaDoc>
{
    public void Configure(EntityTypeBuilder<ThongBaoDaDoc> builder)
    {
        builder.ToTable("thong_bao_da_doc");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ThongBaoId).HasColumnName("thong_bao_id");
        builder.Property(x => x.TaiKhoanId).HasColumnName("tai_khoan_id");
        builder.Property(x => x.DaDoc).HasDefaultValue(false).HasColumnName("da_doc");
        builder.Property(x => x.NgayDoc).HasColumnName("ngay_doc");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.ThongBaoId, x.TaiKhoanId }).IsUnique();
        builder.HasOne(x => x.ThongBao).WithMany(t => t.ThongBaoDaDocs).HasForeignKey(x => x.ThongBaoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.TaiKhoan).WithMany().HasForeignKey(x => x.TaiKhoanId).OnDelete(DeleteBehavior.Cascade);
    }
}
