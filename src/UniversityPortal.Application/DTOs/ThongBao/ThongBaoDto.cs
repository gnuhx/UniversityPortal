namespace UniversityPortal.Application.DTOs.ThongBao;

public class ThongBaoDto
{
    public int Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string LoaiThongBao { get; set; } = string.Empty;
    public string MucDo { get; set; } = string.Empty;
    public int NguoiTaoId { get; set; }
    public string TenNguoiTao { get; set; } = string.Empty;
    public int? LopNhanId { get; set; }
    public string? TenLopNhan { get; set; }
    public DateTime NgayTao { get; set; }
    public bool? DaDoc { get; set; }
}
