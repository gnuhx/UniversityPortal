// DTO cập nhật thông tin sinh viên — cập nhật đồng thời bảng SinhVien và TaiKhoan
namespace UniversityPortal.Application.DTOs.SinhVien;

/// <summary>
/// Dữ liệu đầu vào khi cập nhật thông tin sinh viên.
/// </summary>
public class UpdateSinhVienDto
{
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? LopId { get; set; }
    public bool TrangThai { get; set; }
}
