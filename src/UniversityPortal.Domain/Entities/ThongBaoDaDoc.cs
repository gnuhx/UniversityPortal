using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ThongBaoDaDoc : AuditableEntity
{
    public int ThongBaoId { get; set; }
    public int TaiKhoanId { get; set; }
    public bool DaDoc { get; set; } = false;
    public DateTime? NgayDoc { get; set; }

    public ThongBao ThongBao { get; set; } = null!;
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
