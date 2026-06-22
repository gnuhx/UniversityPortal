using Microsoft.EntityFrameworkCore;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<VaiTro> VaiTros => Set<VaiTro>();
    public DbSet<PhongBan> PhongBans => Set<PhongBan>();
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<GiaoVien> GiaoViens => Set<GiaoVien>();
    public DbSet<SinhVien> SinhViens => Set<SinhVien>();
    public DbSet<KetQuaAnhVanDauVao> KetQuaAnhVanDauVaos => Set<KetQuaAnhVanDauVao>();
    public DbSet<NganhHoc> NganhHocs => Set<NganhHoc>();
    public DbSet<ChuongTrinhDT> ChuongTrinhDTs => Set<ChuongTrinhDT>();
    public DbSet<LopSinhHoat> LopSinhHoats => Set<LopSinhHoat>();
    public DbSet<MonHoc> MonHocs => Set<MonHoc>();
    public DbSet<ChiTietCTDT> ChiTietCTDTs => Set<ChiTietCTDT>();
    public DbSet<NamHoc> NamHocs => Set<NamHoc>();
    public DbSet<HocKy> HocKys => Set<HocKy>();
    public DbSet<TuanHoc> TuanHocs => Set<TuanHoc>();
    public DbSet<LopHocPhan> LopHocPhans => Set<LopHocPhan>();
    public DbSet<DanhSachLopHP> DanhSachLopHPs => Set<DanhSachLopHP>();
    public DbSet<DanhSachThiLai> DanhSachThiLais => Set<DanhSachThiLai>();
    public DbSet<HocBa> HocBas => Set<HocBa>();
    public DbSet<DiemRenLuyen> DiemRenLuyens => Set<DiemRenLuyen>();
    public DbSet<ThoiKhoaBieu> ThoiKhoaBieus => Set<ThoiKhoaBieu>();
    public DbSet<HocPhi> HocPhis => Set<HocPhi>();
    public DbSet<ThongBao> ThongBaos => Set<ThongBao>();
    public DbSet<ThongBaoDaDoc> ThongBaoDaDocs => Set<ThongBaoDaDoc>();
    public DbSet<BinhLuanThongBao> BinhLuanThongBaos => Set<BinhLuanThongBao>();
    public DbSet<BienBanSHCN> BienBanSHCNs => Set<BienBanSHCN>();
    public DbSet<ChiTietCongViec> ChiTietCongViecs => Set<ChiTietCongViec>();
    public DbSet<ChiTietVangSHCN> ChiTietVangSHCNs => Set<ChiTietVangSHCN>();
    public DbSet<YeuCauHanhChinh> YeuCauHanhChinhs => Set<YeuCauHanhChinh>();
    public DbSet<YeuCauSuaDiem> YeuCauSuaDiems => Set<YeuCauSuaDiem>();
    public DbSet<DatPhongThucHanh> DatPhongThucHanhs => Set<DatPhongThucHanh>();
    public DbSet<KhaoSatYKien> KhaoSatYKiens => Set<KhaoSatYKien>();
    public DbSet<DienDanGiaoVien> DienDanGiaoViens => Set<DienDanGiaoVien>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Tự động gán CreatedAt khi thêm mới và UpdatedAt mỗi lần lưu,
    /// cho tất cả entity kế thừa AuditableEntity — không cần set thủ công ở tầng service.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(ct);
    }
}
