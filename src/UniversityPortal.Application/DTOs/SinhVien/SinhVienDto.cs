// DTO phản hồi thông tin sinh viên kèm thông tin tài khoản và lớp sinh hoạt
namespace UniversityPortal.Application.DTOs.SinhVien;

/// <summary>
/// Thông tin sinh viên trả về client, bao gồm thông tin cá nhân và lớp sinh hoạt.
/// </summary>
public class SinhVienDto
{
    public int Id { get; set; }
    public int TaiKhoanId { get; set; }
    public string Mssv { get; set; } = string.Empty;
    public int? LopId { get; set; }
    public string? TenLop { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public bool TrangThai { get; set; }
    public DateTime CreatedAt { get; set; }

    // Ngành / CTĐT của lớp sinh hoạt hiện tại — null nếu sinh viên chưa được phân lớp.
    public int? NganhId { get; set; }
    public string? MaNganh { get; set; }
    public string? TenNganh { get; set; }
    public int? CtdtId { get; set; }
    public string? MaCtdt { get; set; }
    public string? KhoaHoc { get; set; }
}
