using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.LopHocPhan;

namespace UniversityPortal.Application.Interfaces.Services;

public interface ILopHocPhanService
{
    /// <summary>Giáo viên xem tất cả lớp HP mình đang phụ trách.</summary>
    Task<IEnumerable<LopHocPhanDto>> GetByGiaoVienMeAsync(int taiKhoanId);

    /// <summary>Admin/Giáo vụ duyệt danh sách lớp học phần có phân trang, lọc theo học kỳ/từ khoá.</summary>
    Task<PagedResultDto<LopHocPhanDto>> GetPagedAsync(int page, int pageSize, int? hocKyId, string? keyword);

    /// <summary>Khoá bảng điểm (chỉ giáo viên phụ trách LHP đó).</summary>
    Task KhoaBangDiemAsync(int lopHpId, int taiKhoanId);

    /// <summary>Mở khoá bảng điểm (Admin).</summary>
    Task MoBangDiemAsync(int lopHpId);
}
