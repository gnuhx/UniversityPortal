using UniversityPortal.Application.DTOs.BienBanSHCN;
using UniversityPortal.Application.DTOs.Common;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý biên bản sinh hoạt chủ nhiệm.
/// Admin/Giáo vụ xem được mọi lớp; Giáo viên (GVCN) chỉ tạo/xem được lớp mình chủ nhiệm;
/// Sinh viên chỉ xem được lớp mình, và không thấy lý do vắng của bạn khác.
/// </summary>
public interface IBienBanSHCNService
{
    /// <summary>Admin/Giáo vụ: danh sách phân trang, lọc theo lớp (bỏ trống = tất cả lớp).</summary>
    Task<PagedResultDto<BienBanSHCNDto>> GetPagedAsync(int? lopId, int page, int pageSize);

    /// <summary>Admin/Giáo vụ: chi tiết 1 biên bản theo id.</summary>
    Task<BienBanSHCNDto> GetByIdAsync(int id);

    /// <summary>Giáo viên: danh sách phân trang của 1 lớp — ném ForbiddenException nếu không phải GVCN của lớp đó.</summary>
    Task<PagedResultDto<BienBanSHCNDto>> GetPagedForGvcnAsync(int taiKhoanId, int lopId, int page, int pageSize);

    /// <summary>Giáo viên: chi tiết 1 biên bản — ném ForbiddenException nếu không phải GVCN của lớp đó.</summary>
    Task<BienBanSHCNDto> GetDetailForGvcnAsync(int taiKhoanId, int id);

    /// <summary>Giáo viên: tạo biên bản mới cho lớp mình chủ nhiệm, kèm công việc và điểm danh vắng.</summary>
    Task<BienBanSHCNDto> CreateAsync(int taiKhoanId, CreateBienBanSHCNDto dto);

    /// <summary>Sinh viên: danh sách biên bản của lớp mình, ẩn lý do vắng của bạn khác.</summary>
    Task<IEnumerable<BienBanSHCNSinhVienDto>> GetMeAsync(int taiKhoanId);
}
