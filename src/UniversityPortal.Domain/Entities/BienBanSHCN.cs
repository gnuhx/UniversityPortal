using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class BienBanSHCN : AuditableEntity
{
    public int LopId { get; set; }
    public int TuanHocId { get; set; }
    public DateTime ThoiGian { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public int GvcnId { get; set; }
    public int ThuKyId { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public string? PhanHoiGvcn { get; set; }

    public LopSinhHoat Lop { get; set; } = null!;
    public TuanHoc TuanHoc { get; set; } = null!;
    public GiaoVien Gvcn { get; set; } = null!;
    public SinhVien ThuKy { get; set; } = null!;
    public ICollection<ChiTietCongViec> ChiTietCongViecs { get; set; } = [];
    public ICollection<ChiTietVangSHCN> ChiTietVangSHCNs { get; set; } = [];
}
