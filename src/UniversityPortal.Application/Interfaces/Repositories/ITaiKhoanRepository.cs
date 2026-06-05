using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository tài khoản — mở rộng IRepository với các truy vấn đặc thù.
/// </summary>
public interface ITaiKhoanRepository : IRepository<TaiKhoan>
{
    /// <summary>Tìm tài khoản theo tên đăng nhập, include VaiTro.</summary>
    Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap);

    /// <summary>Tìm tài khoản theo refresh token.</summary>
    Task<TaiKhoan?> GetByRefreshTokenAsync(string refreshToken);

    /// <summary>Lấy danh sách tài khoản có phân trang và bộ lọc.</summary>
    /// <param name="keyword">Tìm theo họ tên, email hoặc tên đăng nhập.</param>
    /// <param name="vaiTroId">Lọc theo vai trò (null = tất cả).</param>
    /// <param name="trangThai">Lọc theo trạng thái (null = tất cả).</param>
    Task<PagedResultDto<TaiKhoan>> GetPagedFilterAsync(
        int page, int pageSize,
        string? keyword, int? vaiTroId, bool? trangThai);
}
