// Giao diện dịch vụ quản lý học kỳ
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.HocKy;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý học kỳ.
/// </summary>
public interface IHocKyService
{
    /// <summary>Lấy danh sách học kỳ có phân trang, lọc theo năm học.</summary>
    Task<PagedResultDto<HocKyDto>> GetPagedAsync(int page, int pageSize, int? namHocId);

    /// <summary>Lấy tất cả học kỳ, sắp giảm dần theo ngày bắt đầu (dùng cho dropdown).</summary>
    Task<IEnumerable<HocKyDto>> GetAllAsync();

    /// <summary>Lấy thông tin chi tiết học kỳ theo id.</summary>
    Task<HocKyDto> GetByIdAsync(int id);

    /// <summary>Tạo mới học kỳ.</summary>
    Task<HocKyDto> CreateAsync(UpsertHocKyDto dto);

    /// <summary>Cập nhật học kỳ theo id.</summary>
    Task<HocKyDto> UpdateAsync(int id, UpsertHocKyDto dto);

    /// <summary>Xoá học kỳ; ném BadRequestException nếu còn lớp học phần, chi tiết CTĐT hoặc học phí liên kết.</summary>
    Task DeleteAsync(int id);

    /// <summary>Lấy các học kỳ mà sinh viên (theo taiKhoanId đang đăng nhập) có lớp học phần đã đăng ký.</summary>
    Task<IEnumerable<HocKyDto>> GetForSinhVienMeAsync(int taiKhoanId);

    /// <summary>Lấy các học kỳ mà giáo viên (theo taiKhoanId đang đăng nhập) có lớp học phần đang dạy.</summary>
    Task<IEnumerable<HocKyDto>> GetForGiaoVienMeAsync(int taiKhoanId);
}
