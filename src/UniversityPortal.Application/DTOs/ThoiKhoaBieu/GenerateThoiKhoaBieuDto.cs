namespace UniversityPortal.Application.DTOs.ThoiKhoaBieu;

/// <summary>
/// Yêu cầu tạo hàng loạt buổi học lặp lại hàng tuần cho một lớp học phần,
/// từ tuần bắt đầu đến tuần kết thúc (cùng thứ/tiết/phòng mỗi tuần).
/// </summary>
public class GenerateThoiKhoaBieuDto
{
    public int LopHpId { get; set; }
    public int TuanBatDauId { get; set; }
    public int TuanKetThucId { get; set; }

    /// <summary>Thứ trong tuần: 2 = Thứ Hai ... 7 = Thứ Bảy, 8 = Chủ nhật.</summary>
    public int Thu { get; set; }
    public int TietBatDau { get; set; }
    public int TietKetThuc { get; set; }
    public string PhongHoc { get; set; } = string.Empty;
}
