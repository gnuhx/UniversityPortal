/*
 * PhongBanController — Tra cứu phòng ban / khoa
 * ─────────────────────────────────────────
 * Base route : /api/phong-ban
 * Phân quyền : chỉ đọc, mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET /api/phong-ban/all — Tất cả phòng ban / khoa (dùng dropdown) [Authenticated]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.PhongBan;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/phong-ban")]
[Authorize]
public class PhongBanController(IPhongBanService service) : ControllerBase
{
    /// <summary>
    /// Lấy tất cả phòng ban / khoa không phân trang (dùng để render dropdown select).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<PhongBanDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<PhongBanDto>>.Ok(result));
    }
}
