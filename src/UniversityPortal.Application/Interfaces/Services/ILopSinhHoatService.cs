// Giao diện dịch vụ quản lý lớp sinh hoạt — CRUD, xử lý circular ref thư ký
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.LopSinhHoat;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý lớp sinh hoạt.
/// Lưu ý: khi tạo mới, ThuKyId luôn = null vì sinh viên chưa được gán lớp.
/// Dùng UpdateAsync để gán thư ký sau khi sinh viên đã có lớp.
/// </summary>
public interface ILopSinhHoatService
{
    /// <summary>Lấy danh sách lớp sinh hoạt có phân trang và lọc theo keyword / GVCN.</summary>
    Task<PagedResultDto<LopSinhHoatDto>> GetPagedAsync(int page, int pageSize, string? keyword, int? gvcnId);

    /// <summary>Lấy tất cả lớp sinh hoạt (dùng cho dropdown).</summary>
    Task<IEnumerable<LopSinhHoatDto>> GetAllAsync();

    /// <summary>Lấy thông tin chi tiết lớp sinh hoạt theo id.</summary>
    Task<LopSinhHoatDto> GetByIdAsync(int id);

    /// <summary>Tạo mới lớp sinh hoạt với ThuKyId = null.</summary>
    Task<LopSinhHoatDto> CreateAsync(CreateLopSinhHoatDto dto);

    /// <summary>Cập nhật lớp sinh hoạt; cho phép gán ThuKyId nếu sinh viên đã thuộc lớp.</summary>
    Task<LopSinhHoatDto> UpdateAsync(int id, UpdateLopSinhHoatDto dto);

    /// <summary>Xoá lớp sinh hoạt; ném BadRequestException nếu còn sinh viên trong lớp.</summary>
    Task DeleteAsync(int id);
}
