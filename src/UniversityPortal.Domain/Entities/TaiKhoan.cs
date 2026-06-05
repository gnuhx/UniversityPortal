using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class TaiKhoan : AuditableEntity
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public int VaiTroId { get; set; }
    public int? PhongBanId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public bool TrangThai { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public VaiTro VaiTro { get; set; } = null!;
    public PhongBan? PhongBan { get; set; }
    public GiaoVien? GiaoVien { get; set; }
    public SinhVien? SinhVien { get; set; }
}
