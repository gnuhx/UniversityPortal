// DTO cập nhật thông tin tài khoản — không cho phép đổi tên đăng nhập
namespace UniversityPortal.Application.DTOs.TaiKhoan;

/// <summary>
/// Dữ liệu đầu vào khi cập nhật thông tin tài khoản.
/// Tên đăng nhập không được thay đổi sau khi tạo.
/// </summary>
public class UpdateTaiKhoanDto
{
    public int VaiTroId { get; set; }
    public int? PhongBanId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool TrangThai { get; set; }
}
