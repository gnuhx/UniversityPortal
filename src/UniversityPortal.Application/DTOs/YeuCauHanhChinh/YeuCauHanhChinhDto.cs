namespace UniversityPortal.Application.DTOs.YeuCauHanhChinh;

public class YeuCauHanhChinhDto
{
    public int Id { get; set; }
    public int SinhVienId { get; set; }
    public string TenSinhVien { get; set; } = string.Empty;
    public string Mssv { get; set; } = string.Empty;
    public string LoaiYeuCau { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string? FileDinhKem { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }
    public string? TenNguoiDuyet { get; set; }
    public DateTime NgayTao { get; set; }
    public DateTime CreatedAt { get; set; }
}
