using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class LopHocPhan : AuditableEntity
{
    public int ChiTietCtdtId { get; set; }
    public int HocKyId { get; set; }
    public int GiaoVienId { get; set; }
    public string MaLopHp { get; set; } = string.Empty;
    public bool KhoaBangDiem { get; set; } = false;
    public bool TrangThaiKetThuc { get; set; } = false;

    public ChiTietCTDT ChiTietCTDT { get; set; } = null!;
    public HocKy HocKy { get; set; } = null!;
    public GiaoVien GiaoVien { get; set; } = null!;
    public ICollection<DanhSachLopHP> DanhSachLopHPs { get; set; } = [];
    public ICollection<DanhSachThiLai> DanhSachThiLais { get; set; } = [];
    public ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = [];
    public ICollection<KhaoSatYKien> KhaoSatYKiens { get; set; } = [];
    public ICollection<YeuCauSuaDiem> YeuCauSuaDiems { get; set; } = [];
}
