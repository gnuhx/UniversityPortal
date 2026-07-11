// DTO tạo mới biên bản sinh hoạt chủ nhiệm — GVCN tạo cho lớp mình chủ nhiệm, kèm công việc và điểm danh vắng
namespace UniversityPortal.Application.DTOs.BienBanSHCN;

/// <summary>Dữ liệu đầu vào khi GVCN tạo biên bản sinh hoạt chủ nhiệm cho lớp mình phụ trách.</summary>
public class CreateBienBanSHCNDto
{
    public int LopId { get; set; }
    public int TuanHocId { get; set; }
    public DateTime ThoiGian { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public int ThuKyId { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public string? PhanHoiGvcn { get; set; }
    public List<CongViecInputDto> CongViecs { get; set; } = [];

    /// <summary>Chỉ liệt kê sinh viên vắng — không có trong danh sách này mặc định là có mặt.</summary>
    public List<VangInputDto> DanhSachVang { get; set; } = [];
}

public class CongViecInputDto
{
    public string TenCongViec { get; set; } = string.Empty;
    public string TrangThaiViec { get; set; } = "Chưa thực hiện";
}

public class VangInputDto
{
    public int SinhVienId { get; set; }
    public bool CoPhep { get; set; }
    public string? LyDo { get; set; }
}
