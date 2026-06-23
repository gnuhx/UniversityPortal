using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Auth;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(ApiResponseDto<LoginResponseDto>.Ok(result, "Đăng nhập thành công."));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(ApiResponseDto<LoginResponseDto>.Ok(result, "Làm mới token thành công."));
    }

    /// <summary>
    /// Đăng xuất — thu hồi refresh token của người dùng đang đăng nhập.
    /// Id tài khoản được đọc từ claim NameIdentifier trong JWT, không cần client gửi lên.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<object>>> Logout()
    {
        // Lấy Id tài khoản từ claim trong JWT đang được dùng để gọi request này
        var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(claimId, out var taiKhoanId))
            await authService.RevokeTokenAsync(taiKhoanId);

        return Ok(ApiResponseDto<object>.Ok(null, "Đăng xuất thành công."));
    }
}
