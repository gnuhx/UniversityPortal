namespace UniversityPortal.Application.DTOs.ThoiKhoaBieu;

/// <summary>Kết quả tạo hàng loạt buổi học: số buổi tạo thành công và các tuần bị bỏ qua do trùng phòng/giờ.</summary>
public class GenerateThoiKhoaBieuResultDto
{
    public int SoBuoiDaTao { get; set; }
    public List<string> TuanBiBoQua { get; set; } = [];
}
