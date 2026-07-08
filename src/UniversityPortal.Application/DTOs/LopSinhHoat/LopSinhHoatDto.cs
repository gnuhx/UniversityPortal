// DTO phản hồi thông tin lớp sinh hoạt kèm GVCN, thư ký và CTDT
namespace UniversityPortal.Application.DTOs.LopSinhHoat;

/// <summary>
/// Thông tin lớp sinh hoạt trả về client, bao gồm tên GVCN, thư ký và chương trình đào tạo.
/// </summary>
public class LopSinhHoatDto
{
    public int Id { get; set; }
    public string MaLop { get; set; } = string.Empty;
    public int GvcnId { get; set; }
    public string TenGvcn { get; set; } = string.Empty;
    public int? ThuKyId { get; set; }
    public string? TenThuKy { get; set; }
    public int ChuongTrinhDtId { get; set; }
    public string MaCtdt { get; set; } = string.Empty;
    public string KhoaHoc { get; set; } = string.Empty;
    public int? NganhId { get; set; }
    public string TenNganh { get; set; } = string.Empty;
    public int? PhongBanId { get; set; }
    public string? TenPhongBan { get; set; }
    public int SoSinhVien { get; set; }
    public DateTime CreatedAt { get; set; }
}
