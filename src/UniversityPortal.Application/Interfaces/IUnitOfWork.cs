// Unit of Work — tập hợp tất cả repositories, đảm bảo commit trong một transaction
using UniversityPortal.Application.Interfaces.Repositories;

namespace UniversityPortal.Application.Interfaces;

/// <summary>
/// Đơn vị công việc — cung cấp access đến tất cả repositories và commit một transaction duy nhất.
/// Các service inject IUnitOfWork thay vì inject từng repository riêng lẻ.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // ── Tuần 1 ────────────────────────────────────────────────────────────────
    ITaiKhoanRepository TaiKhoans { get; }
    ISinhVienRepository SinhViens { get; }
    IGiaoVienRepository GiaoViens { get; }
    ILopHocPhanRepository LopHocPhans { get; }
    IDanhSachLopHPRepository DanhSachLopHPs { get; }
    IHocBaRepository HocBas { get; }

    // ── Năm học / Học kỳ ─────────────────────────────────────────────────────
    INamHocRepository NamHocs { get; }
    IHocKyRepository HocKys { get; }
    ITuanHocRepository TuanHocs { get; }

    // ── Thời khoá biểu ────────────────────────────────────────────────────────
    IThoiKhoaBieuRepository ThoiKhoaBieus { get; }

    // ── Tuần 2 — Danh mục ─────────────────────────────────────────────────────
    INganhHocRepository NganhHocs { get; }
    IPhongBanRepository PhongBans { get; }
    IChuongTrinhDTRepository ChuongTrinhDTs { get; }
    IMonHocRepository MonHocs { get; }
    IChiTietCTDTRepository ChiTietCTDTs { get; }
    ILopSinhHoatRepository LopSinhHoats { get; }

    // ── Thông báo ─────────────────────────────────────────────────────────────
    IThongBaoRepository ThongBaos { get; }
    IThongBaoDaDocRepository ThongBaoDaDocs { get; }

    // ── Học phí ───────────────────────────────────────────────────────────────
    IHocPhiRepository HocPhis { get; }

    // ── Yêu cầu ──────────────────────────────────────────────────────────────
    IYeuCauHanhChinhRepository YeuCauHanhChinhs { get; }
    IYeuCauSuaDiemRepository YeuCauSuaDiems { get; }

    // ── Nội dung tĩnh (Thư viện, Học Vụ,...) ─────────────────────────────────
    INoiDungTinhRepository NoiDungTinhs { get; }

    /// <summary>Lưu tất cả thay đổi trong transaction hiện tại xuống database.</summary>
    Task<int> CommitAsync();
}
