// DTO phản hồi thông tin giáo viên kèm thông tin tài khoản liên kết
namespace UniversityPortal.Application.DTOs.GiaoVien;

/// <summary>
/// Thông tin giáo viên trả về client, bao gồm thông tin cá nhân từ tài khoản liên kết.
/// </summary>
public class GiaoVienDto
{
    public int Id { get; set; }
    public int TaiKhoanId { get; set; }
    public string MaGv { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public int? PhongBanId { get; set; }
    public string? TenPhongBan { get; set; }
    public bool TrangThai { get; set; }
    public DateTime CreatedAt { get; set; }
}
