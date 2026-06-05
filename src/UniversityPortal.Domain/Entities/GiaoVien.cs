using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class GiaoVien : AuditableEntity
{
    public int TaiKhoanId { get; set; }
    public string MaGv { get; set; } = string.Empty;

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<LopSinhHoat> LopSinhHoatGVCN { get; set; } = [];
    public ICollection<LopHocPhan> LopHocPhans { get; set; } = [];
    public ICollection<BienBanSHCN> BienBanSHCNs { get; set; } = [];
    public ICollection<DatPhongThucHanh> DatPhongThucHanhs { get; set; } = [];
    public ICollection<DienDanGiaoVien> DienDanGiaoViens { get; set; } = [];
    public ICollection<YeuCauSuaDiem> YeuCauSuaDiems { get; set; } = [];
}
