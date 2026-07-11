// Giao diện dịch vụ quản lý ngành học — CRUD đơn giản, hỗ trợ cấu trúc ngành cha–con
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NganhHoc;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý ngành học.
/// Hỗ trợ cấu trúc cây: một ngành có thể có ngành cha.
/// </summary>
public interface INganhHocService
{
    /// <summary>Lấy danh sách ngành học có phân trang, lọc theo keyword, ngành (id) và khoá học.</summary>
    Task<PagedResultDto<NganhHocDto>> GetPagedAsync(int page, int pageSize, string? keyword, int? nganhId = null, string? khoaHoc = null);

    /// <summary>Lấy tất cả ngành học (dùng cho dropdown).</summary>
    Task<IEnumerable<NganhHocDto>> GetAllAsync();

    /// <summary>Lấy thông tin chi tiết ngành học theo id.</summary>
    Task<NganhHocDto> GetByIdAsync(int id);

    /// <summary>Tạo mới ngành học; ném BadRequestException nếu mã ngành đã tồn tại.</summary>
    Task<NganhHocDto> CreateAsync(UpsertNganhHocDto dto);

    /// <summary>Cập nhật ngành học theo id.</summary>
    Task<NganhHocDto> UpdateAsync(int id, UpsertNganhHocDto dto);

    /// <summary>Xoá ngành học; ném BadRequestException nếu còn CTDT liên kết.</summary>
    Task DeleteAsync(int id);
}
