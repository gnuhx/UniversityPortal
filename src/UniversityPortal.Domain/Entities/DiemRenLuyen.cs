using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class DiemRenLuyen : AuditableEntity
{
    public int SinhVienId { get; set; }
    public int HocKyId { get; set; }
    public int DiemTong { get; set; }
    public string XepLoai { get; set; } = string.Empty;

    public SinhVien SinhVien { get; set; } = null!;
    public HocKy HocKy { get; set; } = null!;
}
