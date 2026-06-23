using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ThongBao;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/thong-bao")]
[Authorize]
public class ThongBaoController(IThongBaoService service) : ControllerBase
{
    /// <summary>Admin/Giáo vụ tạo thông báo mới.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<ThongBaoDto>>> Create([FromBody] CreateThongBaoDto dto)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.CreateAsync(taiKhoanId, dto);
        return Ok(ApiResponseDto<ThongBaoDto>.Ok(result, "Tạo thông báo thành công."));
    }

    /// <summary>Admin xem tất cả thông báo.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ThongBaoDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<ThongBaoDto>>.Ok(result));
    }

    /// <summary>Sinh viên nhận thông báo dành cho mình.</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ThongBaoDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetBySinhVienMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<ThongBaoDto>>.Ok(result));
    }

    /// <summary>Đánh dấu một thông báo là đã đọc.</summary>
    [HttpPut("{id:int}/da-doc")]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<object>>> MarkAsRead(int id)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await service.MarkAsReadAsync(id, taiKhoanId);
        return Ok(ApiResponseDto<object>.Ok(null, "Đã đánh dấu đọc."));
    }

    /// <summary>Admin xoá thông báo.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Đã xoá thông báo."));
    }
}
