using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.YeuCauHanhChinh;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IYeuCauHanhChinhService
{
    /// <summary>Sinh viên xem yêu cầu của mình.</summary>
    Task<IEnumerable<YeuCauHanhChinhDto>> GetByMeAsync(int taiKhoanId);

    /// <summary>Admin/Giáo vụ xem tất cả yêu cầu, lọc theo trạng thái.</summary>
    Task<PagedResultDto<YeuCauHanhChinhDto>> GetAllAsync(int page, int pageSize, string? trangThai);

    /// <summary>Sinh viên tạo yêu cầu mới.</summary>
    Task<YeuCauHanhChinhDto> CreateAsync(int taiKhoanId, CreateYeuCauHanhChinhDto dto);

    /// <summary>Admin/Giáo vụ duyệt hoặc từ chối yêu cầu.</summary>
    Task<YeuCauHanhChinhDto> DuyetAsync(int id, int taiKhoanId, DuyetYeuCauHanhChinhDto dto);
}
