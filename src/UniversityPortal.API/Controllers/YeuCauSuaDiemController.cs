using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.YeuCauSuaDiem;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/yeu-cau-sua-diem")]
[Authorize]
public class YeuCauSuaDiemController(IYeuCauSuaDiemService service) : ControllerBase
{
    /// <summary>Giáo viên xem yêu cầu sửa điểm của mình.</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<YeuCauSuaDiemDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetByMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<YeuCauSuaDiemDto>>.Ok(result));
    }

    /// <summary>Admin xem tất cả yêu cầu sửa điểm.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<YeuCauSuaDiemDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? trangThai = null)
    {
        var result = await service.GetAllAsync(page, pageSize, trangThai);
        return Ok(ApiResponseDto<PagedResultDto<YeuCauSuaDiemDto>>.Ok(result));
    }

    /// <summary>Giáo viên tạo yêu cầu mở khoá bảng điểm.</summary>
    [HttpPost]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<YeuCauSuaDiemDto>>> Create([FromBody] CreateYeuCauSuaDiemDto dto)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.CreateAsync(taiKhoanId, dto);
        return Ok(ApiResponseDto<YeuCauSuaDiemDto>.Ok(result, "Đã gửi yêu cầu mở khoá bảng điểm."));
    }

    /// <summary>Admin duyệt hoặc từ chối — nếu duyệt sẽ tự động mở khoá bảng điểm.</summary>
    [HttpPut("{id:int}/duyet")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<YeuCauSuaDiemDto>>> Duyet(int id, [FromBody] DuyetYeuCauSuaDiemDto dto)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.DuyetAsync(id, taiKhoanId, dto);
        return Ok(ApiResponseDto<YeuCauSuaDiemDto>.Ok(result, $"Đã {dto.TrangThai.ToLower()} yêu cầu."));
    }
}
