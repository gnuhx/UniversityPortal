// DTO cập nhật chi tiết chương trình đào tạo — chỉ cho phép đổi số tín chỉ và cờ tính điểm TB
namespace UniversityPortal.Application.DTOs.ChiTietCTDT;

/// <summary>
/// Dữ liệu đầu vào khi cập nhật thông tin môn học trong chương trình đào tạo.
/// Không cho phép đổi CTDT hay môn học; chỉ cập nhật số tín chỉ và cờ tính điểm.
/// </summary>
public class UpdateChiTietCTDTDto
{
    public int SoTinChi { get; set; }
    public bool TinhDiemTb { get; set; }
}
