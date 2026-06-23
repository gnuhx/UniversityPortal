using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.YeuCauHanhChinh;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/yeu-cau-hanh-chinh")]
[Authorize]
public class YeuCauHanhChinhController(IYeuCauHanhChinhService service) : ControllerBase
{
    /// <summary>Sinh viên xem yêu cầu của mình.</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<YeuCauHanhChinhDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetByMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<YeuCauHanhChinhDto>>.Ok(result));
    }

    /// <summary>Admin/Giáo vụ xem tất cả yêu cầu.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<YeuCauHanhChinhDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? trangThai = null)
    {
        var result = await service.GetAllAsync(page, pageSize, trangThai);
        return Ok(ApiResponseDto<PagedResultDto<YeuCauHanhChinhDto>>.Ok(result));
    }

    /// <summary>Sinh viên tạo yêu cầu hành chính mới.</summary>
    [HttpPost]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<YeuCauHanhChinhDto>>> Create([FromBody] CreateYeuCauHanhChinhDto dto)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.CreateAsync(taiKhoanId, dto);
        return Ok(ApiResponseDto<YeuCauHanhChinhDto>.Ok(result, "Tạo yêu cầu thành công."));
    }

    /// <summary>Admin/Giáo vụ duyệt hoặc từ chối yêu cầu.</summary>
    [HttpPut("{id:int}/duyet")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<YeuCauHanhChinhDto>>> Duyet(int id, [FromBody] DuyetYeuCauHanhChinhDto dto)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.DuyetAsync(id, taiKhoanId, dto);
        return Ok(ApiResponseDto<YeuCauHanhChinhDto>.Ok(result, $"Đã {dto.TrangThai.ToLower()} yêu cầu."));
    }
}
