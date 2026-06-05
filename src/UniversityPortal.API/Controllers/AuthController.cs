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

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<object>>> Logout()
    {
        var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idClaim, out var id))
            await authService.RevokeTokenAsync(id);

        return Ok(ApiResponseDto<object>.Ok(null, "Đăng xuất thành công."));
    }
}
