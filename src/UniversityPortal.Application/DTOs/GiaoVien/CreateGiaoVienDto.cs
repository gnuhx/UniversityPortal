// DTO tạo giáo viên mới — tự động tạo tài khoản kèm theo với vai trò Giáo viên
namespace UniversityPortal.Application.DTOs.GiaoVien;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới giáo viên.
/// Hệ thống sẽ tự động tạo tài khoản đăng nhập tương ứng.
/// </summary>
public class CreateGiaoVienDto
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? PhongBanId { get; set; }
    public string MaGv { get; set; } = string.Empty;
}
