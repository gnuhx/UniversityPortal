// Cài đặt Unit of Work — lazy-init từng repository khi cần, dùng chung AppDbContext
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Infrastructure.Persistence;
using UniversityPortal.Infrastructure.Repositories;

namespace UniversityPortal.Infrastructure;

/// <summary>
/// Cài đặt IUnitOfWork với lazy initialization cho từng repository.
/// Tất cả repositories dùng chung một AppDbContext instance trong vòng đời của request.
/// </summary>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    // ── Tuần 1 ────────────────────────────────────────────────────────────────
    private ITaiKhoanRepository? _taiKhoans;
    private ISinhVienRepository? _sinhViens;
    private IGiaoVienRepository? _giaoViens;
    private ILopHocPhanRepository? _lopHocPhans;
    private IDanhSachLopHPRepository? _danhSachLopHPs;

    // ── Tuần 2 — Danh mục ─────────────────────────────────────────────────────
    private INganhHocRepository? _nganhHocs;
    private IChuongTrinhDTRepository? _chuongTrinhDTs;
    private IMonHocRepository? _monHocs;
    private IChiTietCTDTRepository? _chiTietCTDTs;
    private ILopSinhHoatRepository? _lopSinhHoats;

    public ITaiKhoanRepository TaiKhoans           => _taiKhoans     ??= new TaiKhoanRepository(context);
    public ISinhVienRepository SinhViens           => _sinhViens     ??= new SinhVienRepository(context);
    public IGiaoVienRepository GiaoViens           => _giaoViens     ??= new GiaoVienRepository(context);
    public ILopHocPhanRepository LopHocPhans       => _lopHocPhans   ??= new LopHocPhanRepository(context);
    public IDanhSachLopHPRepository DanhSachLopHPs => _danhSachLopHPs ??= new DanhSachLopHPRepository(context);

    public INganhHocRepository NganhHocs           => _nganhHocs      ??= new NganhHocRepository(context);
    public IChuongTrinhDTRepository ChuongTrinhDTs => _chuongTrinhDTs ??= new ChuongTrinhDTRepository(context);
    public IMonHocRepository MonHocs               => _monHocs        ??= new MonHocRepository(context);
    public IChiTietCTDTRepository ChiTietCTDTs     => _chiTietCTDTs   ??= new ChiTietCTDTRepository(context);
    public ILopSinhHoatRepository LopSinhHoats     => _lopSinhHoats   ??= new LopSinhHoatRepository(context);

    /// <summary>Commit toàn bộ thay đổi của request hiện tại vào database.</summary>
    public Task<int> CommitAsync() => context.SaveChangesAsync();

    public void Dispose() => context.Dispose();
}
