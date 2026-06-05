using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class LopSinhHoat : AuditableEntity
{
    public string MaLop { get; set; } = string.Empty;
    public int GvcnId { get; set; }
    public int? ThuKyId { get; set; }
    public int ChuongTrinhDtId { get; set; }

    public GiaoVien Gvcn { get; set; } = null!;
    public SinhVien? ThuKy { get; set; }
    public ChuongTrinhDT ChuongTrinhDT { get; set; } = null!;
    public ICollection<SinhVien> SinhViens { get; set; } = [];
    public ICollection<ThongBao> ThongBaos { get; set; } = [];
    public ICollection<BienBanSHCN> BienBanSHCNs { get; set; } = [];
    public ICollection<DienDanGiaoVien> DienDanGiaoViens { get; set; } = [];
}
