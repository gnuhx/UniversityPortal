using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Services;

public class JwtTokenService(IConfiguration config) : IJwtTokenService
{
    public string GenerateAccessToken(TaiKhoan taiKhoan)
    {
        var secret  = config["JWT:Secret"]!;
        var issuer  = config["JWT:Issuer"]!;
        var audience = config["JWT:Audience"]!;
        var expiry  = int.Parse(config["JWT:ExpiryMinutes"] ?? "60");

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, taiKhoan.Id.ToString()),
            new Claim(ClaimTypes.Name,           taiKhoan.TenDangNhap),
            new Claim(ClaimTypes.Role,           taiKhoan.VaiTro.TenVaiTro),
            new Claim("ho_ten",                  taiKhoan.HoTen),
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
