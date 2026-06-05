using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class KetQuaAnhVanDauVao : AuditableEntity
{
    public int SinhVienId { get; set; }
    public string HinhThucXet { get; set; } = string.Empty;
    public float? DiemThi { get; set; }
    public float? DiemTa1 { get; set; }
    public float? DiemTa2 { get; set; }
    public float? DiemTa3 { get; set; }
    public string? GhiChu { get; set; }

    public SinhVien SinhVien { get; set; } = null!;
}
