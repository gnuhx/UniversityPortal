using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.YeuCauSuaDiem;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IYeuCauSuaDiemService
{
    /// <summary>Giáo viên xem yêu cầu sửa điểm của mình.</summary>
    Task<IEnumerable<YeuCauSuaDiemDto>> GetByMeAsync(int taiKhoanId);

    /// <summary>Admin xem tất cả yêu cầu sửa điểm.</summary>
    Task<PagedResultDto<YeuCauSuaDiemDto>> GetAllAsync(int page, int pageSize, string? trangThai);

    /// <summary>Giáo viên tạo yêu cầu mở khoá bảng điểm (LHP phải đang bị khoá).</summary>
    Task<YeuCauSuaDiemDto> CreateAsync(int taiKhoanId, CreateYeuCauSuaDiemDto dto);

    /// <summary>Admin duyệt hoặc từ chối. Nếu duyệt → tự động mở khoá bảng điểm LHP.</summary>
    Task<YeuCauSuaDiemDto> DuyetAsync(int id, int taiKhoanId, DuyetYeuCauSuaDiemDto dto);
}
