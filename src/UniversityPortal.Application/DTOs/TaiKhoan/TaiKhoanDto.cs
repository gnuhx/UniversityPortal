// DTO phản hồi thông tin tài khoản — dùng cho tất cả các API trả về dữ liệu tài khoản
namespace UniversityPortal.Application.DTOs.TaiKhoan;

/// <summary>
/// Thông tin tài khoản trả về cho client (không bao gồm mật khẩu và refresh token).
/// </summary>
public class TaiKhoanDto
{
    public int Id { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public int VaiTroId { get; set; }
    public string TenVaiTro { get; set; } = string.Empty;
    public int? PhongBanId { get; set; }
    public string? TenPhongBan { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public bool TrangThai { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
