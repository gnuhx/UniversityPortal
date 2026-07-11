// Giao diện dịch vụ quản lý năm học
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NamHoc;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý năm học.
/// </summary>
public interface INamHocService
{
    /// <summary>Lấy danh sách năm học có phân trang và lọc theo keyword.</summary>
    Task<PagedResultDto<NamHocDto>> GetPagedAsync(int page, int pageSize, string? keyword);

    /// <summary>Lấy tất cả năm học (dùng cho dropdown).</summary>
    Task<IEnumerable<NamHocDto>> GetAllAsync();

    /// <summary>Lấy thông tin chi tiết năm học theo id.</summary>
    Task<NamHocDto> GetByIdAsync(int id);

    /// <summary>Tạo mới năm học; ném BadRequestException nếu tên năm học đã tồn tại.</summary>
    Task<NamHocDto> CreateAsync(UpsertNamHocDto dto);

    /// <summary>Cập nhật năm học theo id.</summary>
    Task<NamHocDto> UpdateAsync(int id, UpsertNamHocDto dto);

    /// <summary>Xoá năm học; ném BadRequestException nếu còn học kỳ hoặc tuần học liên kết.</summary>
    Task DeleteAsync(int id);
}
