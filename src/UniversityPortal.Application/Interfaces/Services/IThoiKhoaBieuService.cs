using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ThoiKhoaBieu;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IThoiKhoaBieuService
{
    Task<PagedResultDto<ThoiKhoaBieuDto>> GetPagedAsync(int page, int pageSize, int? lopHpId);
    Task<ThoiKhoaBieuDto> GetByIdAsync(int id);
    Task<ThoiKhoaBieuDto> CreateAsync(CreateThoiKhoaBieuDto dto);
    Task<ThoiKhoaBieuDto> UpdateAsync(int id, UpdateThoiKhoaBieuDto dto);
    Task DeleteAsync(int id);

    /// <summary>Lịch học của sinh viên đang đăng nhập (theo tài khoản), lọc theo học kỳ nếu có.</summary>
    Task<IEnumerable<ThoiKhoaBieuDto>> GetForSinhVienMeAsync(int taiKhoanId, int? hocKyId);

    /// <summary>Lịch dạy của giáo viên đang đăng nhập (theo tài khoản), lọc theo học kỳ nếu có.</summary>
    Task<IEnumerable<ThoiKhoaBieuDto>> GetForGiaoVienMeAsync(int taiKhoanId, int? hocKyId);
}
