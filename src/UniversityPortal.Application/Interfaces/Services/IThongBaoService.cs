using UniversityPortal.Application.DTOs.ThongBao;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IThongBaoService
{
    /// <summary>Admin/Giáo vụ tạo thông báo mới.</summary>
    Task<ThongBaoDto> CreateAsync(int taiKhoanId, CreateThongBaoDto dto);

    /// <summary>Admin xem tất cả thông báo.</summary>
    Task<IEnumerable<ThongBaoDto>> GetAllAsync();

    /// <summary>Sinh viên nhận thông báo (toàn trường + theo lớp).</summary>
    Task<IEnumerable<ThongBaoDto>> GetBySinhVienMeAsync(int taiKhoanId);

    /// <summary>Đánh dấu thông báo đã đọc.</summary>
    Task MarkAsReadAsync(int thongBaoId, int taiKhoanId);

    /// <summary>Xoá thông báo (Admin).</summary>
    Task DeleteAsync(int thongBaoId);
}
