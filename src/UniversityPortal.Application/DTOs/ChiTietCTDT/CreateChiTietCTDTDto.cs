// DTO tạo mới chi tiết chương trình đào tạo — thêm môn học vào CTDT
namespace UniversityPortal.Application.DTOs.ChiTietCTDT;

/// <summary>
/// Dữ liệu đầu vào khi thêm một môn học vào chương trình đào tạo.
/// </summary>
public class CreateChiTietCTDTDto
{
    public int CtdtId { get; set; }
    public int MonHocId { get; set; }
    public int HocKyId { get; set; }
    public int SoTinChi { get; set; }
    public bool TinhDiemTb { get; set; } = true;
}
