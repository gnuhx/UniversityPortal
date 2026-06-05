// DTO dùng để tạo tài khoản mới — Admin hoặc GiaoVu thực hiện
namespace UniversityPortal.Application.DTOs.TaiKhoan;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới một tài khoản hệ thống.
/// </summary>
public class CreateTaiKhoanDto
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public int VaiTroId { get; set; }
    public int? PhongBanId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
