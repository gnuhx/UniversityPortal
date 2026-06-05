// Giao diện dịch vụ quản lý giáo viên — CRUD kèm tài khoản liên kết
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.GiaoVien;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý giáo viên.
/// Mỗi thao tác tạo/xoá giáo viên đồng thời tác động lên tài khoản liên kết.
/// </summary>
public interface IGiaoVienService
{
    /// <summary>Lấy danh sách giáo viên có phân trang và lọc theo họ tên / mã GV.</summary>
    Task<PagedResultDto<GiaoVienDto>> GetPagedAsync(int page, int pageSize, string? keyword);

    /// <summary>Lấy thông tin chi tiết giáo viên theo id.</summary>
    Task<GiaoVienDto> GetByIdAsync(int id);

    /// <summary>Tạo mới giáo viên kèm tài khoản đăng nhập với vai trò Giáo viên.</summary>
    Task<GiaoVienDto> CreateAsync(CreateGiaoVienDto dto);

    /// <summary>Cập nhật thông tin giáo viên và tài khoản liên kết.</summary>
    Task<GiaoVienDto> UpdateAsync(int id, UpdateGiaoVienDto dto);

    /// <summary>Xoá giáo viên; không xoá tài khoản, chỉ khoá tài khoản liên kết.</summary>
    Task DeleteAsync(int id);
}
