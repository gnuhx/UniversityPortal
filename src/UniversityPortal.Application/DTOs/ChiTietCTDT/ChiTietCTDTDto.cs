// DTO phản hồi chi tiết một môn học trong chương trình đào tạo
namespace UniversityPortal.Application.DTOs.ChiTietCTDT;

/// <summary>
/// Thông tin một môn học cụ thể trong một chương trình đào tạo, bao gồm số tín chỉ và học kỳ.
/// </summary>
public class ChiTietCTDTDto
{
    public int Id { get; set; }
    public int CtdtId { get; set; }
    public string MaCtdt { get; set; } = string.Empty;
    public int MonHocId { get; set; }
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public int HocKyId { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
    public int SoTinChi { get; set; }
    public bool TinhDiemTb { get; set; }
    public DateTime CreatedAt { get; set; }
}
