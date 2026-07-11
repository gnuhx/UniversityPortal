/*
 * TuanHocController — Tra cứu tuần học
 * ─────────────────────────────────────────
 * Base route : /api/tuan-hoc
 * Phân quyền : chỉ đọc, mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET /api/tuan-hoc/all — Tất cả tuần học (dùng dropdown chọn tuần) [Authenticated]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.TuanHoc;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/tuan-hoc")]
[Authorize]
public class TuanHocController(ITuanHocService service) : ControllerBase
{
    /// <summary>
    /// Lấy tất cả tuần học không phân trang (dùng để render dropdown chọn tuần).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<TuanHocDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<TuanHocDto>>.Ok(result));
    }
}
