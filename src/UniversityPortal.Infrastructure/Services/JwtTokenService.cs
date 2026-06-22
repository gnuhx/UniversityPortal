using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Services;

/// <summary>
/// Dịch vụ tạo JWT access token và refresh token.
/// Access token mang thông tin vai trò người dùng; refresh token là chuỗi ngẫu nhiên an toàn.
/// </summary>
public class JwtTokenService(IConfiguration config) : IJwtTokenService
{
    /// <summary>
    /// Tạo JWT access token chứa các claim: Id tài khoản, tên đăng nhập, vai trò, họ tên.
    /// Token được ký bằng HMAC-SHA256 với secret key từ cấu hình JWT:Secret.
    /// </summary>
    public string GenerateAccessToken(TaiKhoan taiKhoan)
    {
        var chuoiBiMat   = config["JWT:Secret"]!;
        var nhaCapToken  = config["JWT:Issuer"]!;
        var doiTuongNhan = config["JWT:Audience"]!;
        var soPhutHetHan = int.Parse(config["JWT:ExpiryMinutes"] ?? "60");

        // Tạo khoá ký từ secret key dạng UTF-8
        var khoaKy    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chuoiBiMat));
        var thongTinKy = new SigningCredentials(khoaKy, SecurityAlgorithms.HmacSha256);

        // Claims nhúng vào token — server không cần tra DB để biết vai trò người dùng
        var danhSachClaim = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, taiKhoan.Id.ToString()),
            new Claim(ClaimTypes.Name,           taiKhoan.TenDangNhap),
            new Claim(ClaimTypes.Role,           taiKhoan.VaiTro.TenVaiTro),
            new Claim("ho_ten",                  taiKhoan.HoTen),
        };

        var token = new JwtSecurityToken(
            issuer:             nhaCapToken,
            audience:           doiTuongNhan,
            claims:             danhSachClaim,
            expires:            DateTime.UtcNow.AddMinutes(soPhutHetHan),
            signingCredentials: thongTinKy);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Tạo refresh token là chuỗi Base64 ngẫu nhiên 64 byte (512 bit).
    /// Dùng RandomNumberGenerator để đảm bảo tính ngẫu nhiên an toàn về mật mã.
    /// </summary>
    public string GenerateRefreshToken()
    {
        var byteNgauNhien = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(byteNgauNhien);
    }
}
