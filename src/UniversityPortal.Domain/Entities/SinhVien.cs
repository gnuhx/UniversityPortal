using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class SinhVien : AuditableEntity
{
    public int TaiKhoanId { get; set; }
    public string Mssv { get; set; } = string.Empty;
    public int? LopId { get; set; }

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public LopSinhHoat? Lop { get; set; }
    public KetQuaAnhVanDauVao? KetQuaAnhVanDauVao { get; set; }
    public ICollection<DanhSachLopHP> DanhSachLopHPs { get; set; } = [];
    public ICollection<DanhSachThiLai> DanhSachThiLais { get; set; } = [];
    public ICollection<HocBa> HocBas { get; set; } = [];
    public ICollection<DiemRenLuyen> DiemRenLuyens { get; set; } = [];
    public ICollection<HocPhi> HocPhis { get; set; } = [];
    public ICollection<YeuCauHanhChinh> YeuCauHanhChinhs { get; set; } = [];
    public ICollection<KhaoSatYKien> KhaoSatYKiens { get; set; } = [];
    public ICollection<ChiTietVangSHCN> ChiTietVangSHCNs { get; set; } = [];
    public ICollection<LopSinhHoat> LopSinhHoatThuKy { get; set; } = [];
    public ICollection<BienBanSHCN> BienBanSHCNThuKy { get; set; } = [];
}
