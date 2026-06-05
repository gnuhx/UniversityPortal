using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository giáo viên — mở rộng IRepository với các truy vấn đặc thù.
/// </summary>
public interface IGiaoVienRepository : IRepository<GiaoVien>
{
    /// <summary>Tìm giáo viên theo tài khoản liên kết, include TaiKhoan.</summary>
    Task<GiaoVien?> GetByTaiKhoanIdAsync(int taiKhoanId);

    /// <summary>Tìm giáo viên theo mã GV.</summary>
    Task<GiaoVien?> GetByMaGvAsync(string maGv);

    /// <summary>Lấy danh sách giáo viên có phân trang và lọc theo keyword (họ tên, mã GV).</summary>
    Task<PagedResultDto<GiaoVien>> GetPagedFilterAsync(int page, int pageSize, string? keyword);

    /// <summary>Lấy thông tin giáo viên theo id, include TaiKhoan và PhongBan.</summary>
    Task<GiaoVien?> GetDetailAsync(int id);
}
