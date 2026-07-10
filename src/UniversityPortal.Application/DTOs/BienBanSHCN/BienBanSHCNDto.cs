// DTO chi tiết biên bản sinh hoạt chủ nhiệm — dùng cho Admin/Giáo vụ/GVCN (thấy toàn bộ danh sách vắng của lớp)
namespace UniversityPortal.Application.DTOs.BienBanSHCN;

/// <summary>
/// Thông tin đầy đủ 1 biên bản sinh hoạt chủ nhiệm, bao gồm danh sách công việc và điểm danh vắng cả lớp.
/// </summary>
public class BienBanSHCNDto
{
    public int Id { get; set; }
    public int LopId { get; set; }
    public string MaLop { get; set; } = string.Empty;
    public int TuanHocId { get; set; }
    public string MaTuan { get; set; } = string.Empty;
    public DateTime ThoiGian { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public int GvcnId { get; set; }
    public string TenGvcn { get; set; } = string.Empty;
    public int ThuKyId { get; set; }
    public string TenThuKy { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string? PhanHoiGvcn { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CongViecItemDto> CongViecs { get; set; } = [];
    public List<VangItemDto> DanhSachVang { get; set; } = [];
}

/// <summary>1 công việc được giao/ghi nhận trong buổi sinh hoạt.</summary>
public class CongViecItemDto
{
    public int Id { get; set; }
    public string TenCongViec { get; set; } = string.Empty;
    public string TrangThaiViec { get; set; } = string.Empty;
}

/// <summary>1 sinh viên vắng trong buổi sinh hoạt (chỉ dùng cho DTO đầy đủ — Admin/GVCN).</summary>
public class VangItemDto
{
    public int SinhVienId { get; set; }
    public string Mssv { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public bool CoPhep { get; set; }
    public string? LyDo { get; set; }
}
