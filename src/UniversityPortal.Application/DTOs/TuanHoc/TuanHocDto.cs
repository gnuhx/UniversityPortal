// DTO tra cứu tuần học — dùng cho dropdown chọn tuần khi tạo/sửa thời khoá biểu
namespace UniversityPortal.Application.DTOs.TuanHoc;

public class TuanHocDto
{
    public int Id { get; set; }
    public string MaTuan { get; set; } = string.Empty;
    public int SoThuTuTuan { get; set; }
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public string TenNamHoc { get; set; } = string.Empty;
}
