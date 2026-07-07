using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class NoiDungTinhConfiguration : IEntityTypeConfiguration<NoiDungTinh>
{
    public void Configure(EntityTypeBuilder<NoiDungTinh> builder)
    {
        builder.ToTable("noi_dung_tinh");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.KhuVuc).IsRequired().HasMaxLength(50).HasColumnName("khu_vuc");
        builder.Property(x => x.MaMuc).IsRequired().HasMaxLength(100).HasColumnName("ma_muc");
        builder.Property(x => x.TieuDe).IsRequired().HasMaxLength(200).HasColumnName("tieu_de");
        builder.Property(x => x.NoiDung).HasColumnType("nvarchar(max)").HasColumnName("noi_dung");
        builder.Property(x => x.ThuTu).HasColumnName("thu_tu");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.KhuVuc, x.ThuTu });
    }
}
