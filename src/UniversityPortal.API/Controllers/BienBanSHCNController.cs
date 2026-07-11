/*
 * BienBanSHCNController — Biên bản sinh hoạt chủ nhiệm
 * ──────────────────────────────────────────────────────
 * Base route : /api/bien-ban-shcn
 * Phân quyền : Admin/Giáo vụ (xem mọi lớp), Giáo viên (GVCN — tạo & xem lớp mình chủ nhiệm),
 *              Sinh viên (xem lớp mình, không thấy lý do vắng của bạn khác)
 *
 * Danh sách endpoint:
 *   GET  /api/bien-ban-shcn                — Danh sách phân trang, lọc lopId     [Admin, Giáo vụ]
 *   GET  /api/bien-ban-shcn/{id}           — Chi tiết                            [Admin, Giáo vụ]
 *   GET  /api/bien-ban-shcn/me-gvcn        — Danh sách của 1 lớp mình chủ nhiệm  [Giáo viên]
 *   GET  /api/bien-ban-shcn/me-gvcn/{id}   — Chi tiết (kiểm tra sở hữu)          [Giáo viên]
 *   POST /api/bien-ban-shcn                — Tạo biên bản cho lớp mình chủ nhiệm [Giáo viên]
 *   GET  /api/bien-ban-shcn/me             — Danh sách biên bản của lớp mình     [Sinh viên]
 */
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.BienBanSHCN;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/bien-ban-shcn")]
[Authorize]
public class BienBanSHCNController(IBienBanSHCNService service) : ControllerBase
{
    /// <summary>Admin/Giáo vụ: danh sách phân trang, lọc theo lớp (bỏ trống = tất cả lớp).</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<BienBanSHCNDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? lopId = null)
    {
        var result = await service.GetPagedAsync(lopId, page, pageSize);
        return Ok(ApiResponseDto<PagedResultDto<BienBanSHCNDto>>.Ok(result));
    }

    /// <summary>Admin/Giáo vụ: chi tiết 1 biên bản theo id.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<BienBanSHCNDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<BienBanSHCNDto>.Ok(result));
    }

    /// <summary>Giáo viên: danh sách phân trang của 1 lớp mình chủ nhiệm.</summary>
    [HttpGet("me-gvcn")]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<BienBanSHCNDto>>>> GetPagedForGvcn(
        [FromQuery] int lopId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetPagedForGvcnAsync(taiKhoanId, lopId, page, pageSize);
        return Ok(ApiResponseDto<PagedResultDto<BienBanSHCNDto>>.Ok(result));
    }

    /// <summary>Giáo viên: chi tiết 1 biên bản của lớp mình chủ nhiệm.</summary>
    [HttpGet("me-gvcn/{id:int}")]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<BienBanSHCNDto>>> GetDetailForGvcn(int id)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetDetailForGvcnAsync(taiKhoanId, id);
        return Ok(ApiResponseDto<BienBanSHCNDto>.Ok(result));
    }

    /// <summary>Giáo viên: tạo biên bản mới cho lớp mình chủ nhiệm.</summary>
    [HttpPost]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<BienBanSHCNDto>>> Create([FromBody] CreateBienBanSHCNDto dto)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.CreateAsync(taiKhoanId, dto);
        return CreatedAtAction(nameof(GetDetailForGvcn), new { id = result.Id },
            ApiResponseDto<BienBanSHCNDto>.Ok(result, "Tạo biên bản sinh hoạt thành công."));
    }

    /// <summary>Sinh viên: danh sách biên bản của lớp mình (ẩn lý do vắng của bạn khác).</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<BienBanSHCNSinhVienDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<BienBanSHCNSinhVienDto>>.Ok(result));
    }
}
