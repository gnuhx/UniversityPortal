using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class NamHocConfiguration : IEntityTypeConfiguration<NamHoc>
{
    public void Configure(EntityTypeBuilder<NamHoc> builder)
    {
        builder.ToTable("nam_hoc");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TenNamHoc).IsRequired().HasMaxLength(20).HasColumnName("ten_nam_hoc");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
