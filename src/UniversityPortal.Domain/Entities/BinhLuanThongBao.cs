using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class BinhLuanThongBao : AuditableEntity
{
    public int ThongBaoId { get; set; }
    public int TaiKhoanId { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayBinhLuan { get; set; }

    public ThongBao ThongBao { get; set; } = null!;
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
