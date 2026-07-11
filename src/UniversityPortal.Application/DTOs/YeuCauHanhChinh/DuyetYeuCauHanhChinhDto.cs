namespace UniversityPortal.Application.DTOs.YeuCauHanhChinh;

public class DuyetYeuCauHanhChinhDto
{
    public string TrangThai { get; set; } = string.Empty; // "Đã duyệt" | "Từ chối"
    public string? GhiChu { get; set; }
}
