// DTO kết quả nhân bản CTDT — số môn đã sao chép và danh sách môn bị bỏ qua kèm lý do
namespace UniversityPortal.Application.DTOs.ChuongTrinhDT;

public class CloneChuongTrinhDTResultDto
{
    public int CtdtMoiId { get; set; }
    public string MaCtdtMoi { get; set; } = string.Empty;
    public int SoMonDaSaoChep { get; set; }
    public List<string> MonBoQua { get; set; } = [];
}
