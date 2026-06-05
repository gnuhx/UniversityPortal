// DTO tạo lớp sinh hoạt mới — thư ký để trống vì sinh viên chưa được gán lớp
namespace UniversityPortal.Application.DTOs.LopSinhHoat;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới lớp sinh hoạt.
/// ThuKyId không được truyền lúc tạo do sinh viên chưa có lớp; cập nhật sau qua UpdateLopSinhHoatDto.
/// </summary>
public class CreateLopSinhHoatDto
{
    public string MaLop { get; set; } = string.Empty;
    public int GvcnId { get; set; }
    public int ChuongTrinhDtId { get; set; }
}
