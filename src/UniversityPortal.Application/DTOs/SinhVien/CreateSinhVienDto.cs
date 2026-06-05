// DTO tạo sinh viên mới — tự động tạo tài khoản kèm theo với vai trò Sinh viên
namespace UniversityPortal.Application.DTOs.SinhVien;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới sinh viên.
/// Hệ thống sẽ tự động tạo tài khoản đăng nhập tương ứng.
/// </summary>
public class CreateSinhVienDto
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mssv { get; set; } = string.Empty;
    public int? LopId { get; set; }
}
