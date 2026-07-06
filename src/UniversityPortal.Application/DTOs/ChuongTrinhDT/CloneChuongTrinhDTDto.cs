// DTO yêu cầu nhân bản CTDT của 1 ngành sang khoá học mới
namespace UniversityPortal.Application.DTOs.ChuongTrinhDT;

public class CloneChuongTrinhDTDto
{
    public int NganhId { get; set; }
    public string KhoaHocMoi { get; set; } = string.Empty;
    public string MaCtdtMoi { get; set; } = string.Empty;
}
