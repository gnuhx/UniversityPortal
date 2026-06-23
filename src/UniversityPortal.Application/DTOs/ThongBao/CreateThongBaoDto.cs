namespace UniversityPortal.Application.DTOs.ThongBao;

public class CreateThongBaoDto
{
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string LoaiThongBao { get; set; } = "Thông báo chung";
    public string MucDo { get; set; } = "Thường";
    public int? LopNhanId { get; set; }
}
