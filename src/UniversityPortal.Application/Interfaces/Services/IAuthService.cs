using UniversityPortal.Application.DTOs.Auth;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(int taiKhoanId);
}
