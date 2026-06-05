using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ThongBao : AuditableEntity
{
    public string LoaiThongBao { get; set; } = string.Empty;
    public string MucDo { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public int NguoiTaoId { get; set; }
    public int? LopNhanId { get; set; }
    public DateTime NgayTao { get; set; }

    public TaiKhoan NguoiTao { get; set; } = null!;
    public LopSinhHoat? LopNhan { get; set; }
    public ICollection<ThongBaoDaDoc> ThongBaoDaDocs { get; set; } = [];
    public ICollection<BinhLuanThongBao> BinhLuans { get; set; } = [];
}
