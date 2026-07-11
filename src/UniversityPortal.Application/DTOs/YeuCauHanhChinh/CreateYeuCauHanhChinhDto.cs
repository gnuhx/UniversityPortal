namespace UniversityPortal.Application.DTOs.YeuCauHanhChinh;

public class CreateYeuCauHanhChinhDto
{
    public string LoaiYeuCau { get; set; } = string.Empty;
    public string? LoaiGiayXacNhan { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public string? FileDinhKem { get; set; }
}
