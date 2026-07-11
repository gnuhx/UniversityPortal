using Microsoft.Extensions.Configuration;
using UniversityPortal.Application.DTOs.Auth;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Dịch vụ xác thực người dùng — đăng nhập, làm mới token và thu hồi token.
/// </summary>
public class AuthService(
    IUnitOfWork uow,
    IJwtTokenService jwtTokenService,
    IConfiguration configuration) : IAuthService
{
    /// <summary>
    /// Đăng nhập và trả về cặp access token / refresh token.
    /// Thứ tự kiểm tra: tài khoản tồn tại → tài khoản đang hoạt động → mật khẩu đúng.
    /// </summary>
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Kiểm tra tài khoản tồn tại; dùng cùng thông báo lỗi để không lộ tên đăng nhập
        var taiKhoan = await uow.TaiKhoans.GetByTenDangNhapAsync(request.TenDangNhap)
            ?? throw new BadRequestException("Tên đăng nhập hoặc mật khẩu không đúng.");

        if (!taiKhoan.TrangThai)
            throw new ForbiddenException("Tài khoản đã bị khoá.");

        if (!BCrypt.Net.BCrypt.Verify(request.MatKhau, taiKhoan.MatKhau))
            throw new BadRequestException("Tên đăng nhập hoặc mật khẩu không đúng.");

        var accessToken  = jwtTokenService.GenerateAccessToken(taiKhoan);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        // Lưu refresh token và thời hạn vào database để dùng cho lần làm mới sau
        var soNgayRefresh = int.Parse(configuration["JWT:RefreshTokenExpiryDays"] ?? "7");
        taiKhoan.RefreshToken       = refreshToken;
        taiKhoan.RefreshTokenExpiry = DateTime.UtcNow.AddDays(soNgayRefresh);
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

    /// <summary>
    /// Xoay vòng refresh token — vô hiệu hoá token cũ, cấp cặp token mới.
    /// Token cũ không thể dùng lại sau khi gọi phương thức này.
    /// </summary>
    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var taiKhoan = await uow.TaiKhoans.GetByRefreshTokenAsync(refreshToken)
            ?? throw new BadRequestException("Refresh token không hợp lệ.");

        if (taiKhoan.RefreshTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Refresh token đã hết hạn. Vui lòng đăng nhập lại.");

        var accessTokenMoi  = jwtTokenService.GenerateAccessToken(taiKhoan);
        var refreshTokenMoi = jwtTokenService.GenerateRefreshToken();

        // Xoay vòng: ghi đè token cũ bằng token mới (token cũ bị vô hiệu hoá)
        var soNgayRefresh = int.Parse(configuration["JWT:RefreshTokenExpiryDays"] ?? "7");
        taiKhoan.RefreshToken       = refreshTokenMoi;
        taiKhoan.RefreshTokenExpiry = DateTime.UtcNow.AddDays(soNgayRefresh);
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();

        return new LoginResponseDto
        {
            AccessToken  = accessTokenMoi,
            RefreshToken = refreshTokenMoi,
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

    /// <summary>
    /// Thu hồi refresh token của tài khoản — người dùng sẽ phải đăng nhập lại.
    /// Được gọi khi người dùng đăng xuất.
    /// </summary>
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
