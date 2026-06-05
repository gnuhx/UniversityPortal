// DTO cập nhật thông tin giáo viên — cập nhật đồng thời bảng GiaoVien và TaiKhoan
namespace UniversityPortal.Application.DTOs.GiaoVien;

/// <summary>
/// Dữ liệu đầu vào khi cập nhật thông tin giáo viên.
/// </summary>
public class UpdateGiaoVienDto
{
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MaGv { get; set; } = string.Empty;
    public int? PhongBanId { get; set; }
    public bool TrangThai { get; set; }
}
