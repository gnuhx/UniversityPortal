// Giao diện dịch vụ quản lý chương trình đào tạo
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ChuongTrinhDT;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý chương trình đào tạo (CTDT).
/// </summary>
public interface IChuongTrinhDTService
{
    /// <summary>Lấy danh sách CTDT có phân trang và lọc theo keyword, ngành và khoá học.</summary>
    Task<PagedResultDto<ChuongTrinhDTDto>> GetPagedAsync(int page, int pageSize, string? keyword, int? nganhId, string? khoaHoc);

    /// <summary>Lấy tất cả CTDT (dùng cho dropdown).</summary>
    Task<IEnumerable<ChuongTrinhDTDto>> GetAllAsync();

    /// <summary>Lấy thông tin chi tiết CTDT theo id.</summary>
    Task<ChuongTrinhDTDto> GetByIdAsync(int id);

    /// <summary>Tạo mới CTDT; ném BadRequestException nếu mã CTDT đã tồn tại.</summary>
    Task<ChuongTrinhDTDto> CreateAsync(UpsertChuongTrinhDTDto dto);

    /// <summary>Cập nhật CTDT theo id.</summary>
    Task<ChuongTrinhDTDto> UpdateAsync(int id, UpsertChuongTrinhDTDto dto);

    /// <summary>Xoá CTDT; ném BadRequestException nếu còn lớp sinh hoạt liên kết.</summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Nhân bản CTDT mới nhất của 1 ngành sang khoá học mới — tạo CTDT mới và sao chép
    /// toàn bộ môn học, ánh xạ đúng học kỳ tương ứng theo năm/thứ tự trong khoá học mới.
    /// </summary>
    Task<CloneChuongTrinhDTResultDto> CloneAsync(CloneChuongTrinhDTDto dto);
}
