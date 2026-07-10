// DTO biên bản sinh hoạt chủ nhiệm dành riêng cho Sinh viên — không lộ lý do vắng của bạn khác trong lớp
namespace UniversityPortal.Application.DTOs.BienBanSHCN;

/// <summary>
/// Thông tin 1 biên bản sinh hoạt chủ nhiệm hiển thị cho sinh viên: nội dung buổi họp đầy đủ,
/// nhưng phần điểm danh chỉ có tình trạng của chính sinh viên đang xem — không có lý do vắng của bạn khác.
/// </summary>
public class BienBanSHCNSinhVienDto
{
    public int Id { get; set; }
    public int TuanHocId { get; set; }
    public string MaTuan { get; set; } = string.Empty;
    public DateTime ThoiGian { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public string TenGvcn { get; set; } = string.Empty;
    public string TenThuKy { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string? PhanHoiGvcn { get; set; }
    public List<CongViecItemDto> CongViecs { get; set; } = [];

    /// <summary>"CoMat" | "VangCoPhep" | "VangKhongPhep".</summary>
    public string TinhTrangCuaToi { get; set; } = string.Empty;
    public string? LyDoVangCuaToi { get; set; }
}
