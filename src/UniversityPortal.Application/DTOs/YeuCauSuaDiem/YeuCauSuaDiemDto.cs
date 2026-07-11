namespace UniversityPortal.Application.DTOs.YeuCauSuaDiem;

public class YeuCauSuaDiemDto
{
    public int Id { get; set; }
    public int LopHpId { get; set; }
    public string MaLopHp { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public string TenHocKy { get; set; } = string.Empty;
    public int GiaoVienId { get; set; }
    public string TenGiaoVien { get; set; } = string.Empty;
    public string LyDo { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }
    public string? TenNguoiDuyet { get; set; }
    public DateTime CreatedAt { get; set; }
}
