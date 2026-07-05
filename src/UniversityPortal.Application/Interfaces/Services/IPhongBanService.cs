using UniversityPortal.Application.DTOs.PhongBan;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ tra cứu phòng ban / khoa — chỉ đọc, dùng cho dropdown.
/// </summary>
public interface IPhongBanService
{
    /// <summary>Lấy tất cả phòng ban / khoa (không phân trang, dùng cho dropdown).</summary>
    Task<IEnumerable<PhongBanDto>> GetAllAsync();
}
