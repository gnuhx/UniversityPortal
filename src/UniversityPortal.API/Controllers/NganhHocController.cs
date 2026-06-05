/*
 * NganhHocController — Quản lý ngành học
 * ─────────────────────────────────────────
 * Base route : /api/nganh-hoc
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/nganh-hoc        — Danh sách (phân trang + lọc)   [Authenticated]
 *   GET    /api/nganh-hoc/all    — Tất cả ngành (dùng dropdown)   [Authenticated]
 *   GET    /api/nganh-hoc/{id}   — Chi tiết ngành học             [Authenticated]
 *   POST   /api/nganh-hoc        — Tạo mới ngành học              [Admin]
 *   PUT    /api/nganh-hoc/{id}   — Cập nhật ngành học             [Admin]
 *   DELETE /api/nganh-hoc/{id}   — Xoá ngành học                 [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NganhHoc;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/nganh-hoc")]
[Authorize]
public class NganhHocController(INganhHocService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách ngành học có phân trang, lọc theo keyword (mã/tên ngành).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<NganhHocDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword);
        return Ok(ApiResponseDto<PagedResultDto<NganhHocDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy tất cả ngành học không phân trang (dùng để render dropdown select).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<NganhHocDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<NganhHocDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết ngành học theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<NganhHocDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<NganhHocDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới ngành học.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<NganhHocDto>>> Create([FromBody] UpsertNganhHocDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<NganhHocDto>.Ok(result, "Tạo ngành học thành công."));
    }

    /// <summary>
    /// Cập nhật ngành học theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<NganhHocDto>>> Update(int id, [FromBody] UpsertNganhHocDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<NganhHocDto>.Ok(result, "Cập nhật ngành học thành công."));
    }

    /// <summary>
    /// Xoá ngành học. Sẽ thất bại nếu còn CTDT đang liên kết.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá ngành học thành công."));
    }
}
