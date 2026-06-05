using Microsoft.Extensions.Configuration;
using UniversityPortal.Application.DTOs.Auth;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class AuthService(
    IUnitOfWork uow,
    IJwtTokenService jwt,
    IConfiguration configuration) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var taiKhoan = await uow.TaiKhoans.GetByTenDangNhapAsync(request.TenDangNhap)
            ?? throw new BadRequestException("Tên đăng nhập hoặc mật khẩu không đúng.");

        if (!taiKhoan.TrangThai)
            throw new ForbiddenException("Tài khoản đã bị khoá.");

        if (!BCrypt.Net.BCrypt.Verify(request.MatKhau, taiKhoan.MatKhau))
            throw new BadRequestException("Tên đăng nhập hoặc mật khẩu không đúng.");

        var accessToken  = jwt.GenerateAccessToken(taiKhoan);
        var refreshToken = jwt.GenerateRefreshToken();

        var refreshDays = int.Parse(configuration["JWT:RefreshTokenExpiryDays"] ?? "7");
        taiKhoan.RefreshToken       = refreshToken;
        taiKhoan.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();

        return new LoginResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken,
            UserInfo     = new UserInfoDto
            {
                Id          = taiKhoan.Id,
                HoTen       = taiKhoan.HoTen,
                Email       = taiKhoan.Email,
                VaiTro      = taiKhoan.VaiTro.TenVaiTro,
                AnhDaiDien  = taiKhoan.AnhDaiDien
            }
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var taiKhoan = await uow.TaiKhoans.GetByRefreshTokenAsync(refreshToken)
            ?? throw new BadRequestException("Refresh token không hợp lệ.");

        if (taiKhoan.RefreshTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Refresh token đã hết hạn. Vui lòng đăng nhập lại.");

        var newAccessToken  = jwt.GenerateAccessToken(taiKhoan);
        var newRefreshToken = jwt.GenerateRefreshToken();

        var refreshDays = int.Parse(configuration["JWT:RefreshTokenExpiryDays"] ?? "7");
        taiKhoan.RefreshToken       = newRefreshToken;
        taiKhoan.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();

        return new LoginResponseDto
        {
            AccessToken  = newAccessToken,
            RefreshToken = newRefreshToken,
            UserInfo     = new UserInfoDto
            {
                Id         = taiKhoan.Id,
                HoTen      = taiKhoan.HoTen,
                Email      = taiKhoan.Email,
                VaiTro     = taiKhoan.VaiTro.TenVaiTro,
                AnhDaiDien = taiKhoan.AnhDaiDien
            }
        };
    }

    public async Task RevokeTokenAsync(int taiKhoanId)
    {
        var taiKhoan = await uow.TaiKhoans.GetByIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Tài khoản không tồn tại.");

        taiKhoan.RefreshToken       = null;
        taiKhoan.RefreshTokenExpiry = null;
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();
    }
}
