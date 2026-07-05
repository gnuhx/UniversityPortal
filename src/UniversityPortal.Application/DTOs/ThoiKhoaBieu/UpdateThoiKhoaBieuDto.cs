namespace UniversityPortal.Application.DTOs.ThoiKhoaBieu;

public class UpdateThoiKhoaBieuDto
{
    public int TuanHocId { get; set; }

    /// <summary>Thứ trong tuần: 2 = Thứ Hai ... 7 = Thứ Bảy, 8 = Chủ nhật.</summary>
    public int Thu { get; set; }
    public int TietBatDau { get; set; }
    public int TietKetThuc { get; set; }
    public string PhongHoc { get; set; } = string.Empty;
}
