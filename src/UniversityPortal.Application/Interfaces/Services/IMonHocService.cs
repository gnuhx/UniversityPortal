// Giao diện dịch vụ quản lý môn học
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.MonHoc;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý môn học.
/// </summary>
public interface IMonHocService
{
    /// <summary>Lấy danh sách môn học có phân trang và lọc theo keyword (mã, tên môn).</summary>
    Task<PagedResultDto<MonHocDto>> GetPagedAsync(int page, int pageSize, string? keyword);

    /// <summary>Lấy tất cả môn học (dùng cho dropdown chọn môn trong chi tiết CTDT).</summary>
    Task<IEnumerable<MonHocDto>> GetAllAsync();

    /// <summary>Lấy thông tin chi tiết môn học theo id.</summary>
    Task<MonHocDto> GetByIdAsync(int id);

    /// <summary>Tạo mới môn học; ném BadRequestException nếu mã môn đã tồn tại.</summary>
    Task<MonHocDto> CreateAsync(UpsertMonHocDto dto);

    /// <summary>Cập nhật môn học theo id.</summary>
    Task<MonHocDto> UpdateAsync(int id, UpsertMonHocDto dto);

    /// <summary>Xoá môn học; ném BadRequestException nếu còn trong chi tiết CTDT.</summary>
    Task DeleteAsync(int id);
}
