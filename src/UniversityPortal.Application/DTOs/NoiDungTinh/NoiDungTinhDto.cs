namespace UniversityPortal.Application.DTOs.NoiDungTinh;

public class NoiDungTinhDto
{
    public int Id { get; set; }
    public string KhuVuc { get; set; } = string.Empty;
    public string MaMuc { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public int ThuTu { get; set; }
    public DateTime CreatedAt { get; set; }
}
