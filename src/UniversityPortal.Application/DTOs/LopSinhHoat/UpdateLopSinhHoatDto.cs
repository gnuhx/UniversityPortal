// DTO cập nhật lớp sinh hoạt — cho phép gán thư ký sau khi sinh viên đã có lớp
namespace UniversityPortal.Application.DTOs.LopSinhHoat;

/// <summary>
/// Dữ liệu đầu vào khi cập nhật lớp sinh hoạt.
/// ThuKyId được cập nhật riêng sau khi sinh viên đã được gán vào lớp để tránh circular reference.
/// </summary>
public class UpdateLopSinhHoatDto
{
    public string MaLop { get; set; } = string.Empty;
    public int GvcnId { get; set; }
    public int? ThuKyId { get; set; }
    public int ChuongTrinhDtId { get; set; }
}
