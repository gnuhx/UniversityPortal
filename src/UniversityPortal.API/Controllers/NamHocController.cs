/*
 * NamHocController — Quản lý năm học
 * ─────────────────────────────────────────
 * Base route : /api/nam-hoc
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/nam-hoc        — Danh sách (phân trang + lọc keyword)  [Authenticated]
 *   GET    /api/nam-hoc/all    — Tất cả năm học (dùng dropdown)        [Authenticated]
 *   GET    /api/nam-hoc/{id}   — Chi tiết năm học                      [Authenticated]
 *   POST   /api/nam-hoc        — Tạo mới năm học                       [Admin]
 *   PUT    /api/nam-hoc/{id}   — Cập nhật năm học                      [Admin]
 *   DELETE /api/nam-hoc/{id}   — Xoá năm học                          [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NamHoc;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/nam-hoc")]
[Authorize]
public class NamHocController(INamHocService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách năm học có phân trang, lọc theo keyword (tên năm học).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<NamHocDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword);
        return Ok(ApiResponseDto<PagedResultDto<NamHocDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy tất cả năm học không phân trang (dùng để render dropdown select).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<NamHocDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<NamHocDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết năm học theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<NamHocDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<NamHocDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới năm học.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<NamHocDto>>> Create([FromBody] UpsertNamHocDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<NamHocDto>.Ok(result, "Tạo năm học thành công."));
    }

    /// <summary>
    /// Cập nhật năm học theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<NamHocDto>>> Update(int id, [FromBody] UpsertNamHocDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<NamHocDto>.Ok(result, "Cập nhật năm học thành công."));
    }

    /// <summary>
    /// Xoá năm học. Sẽ thất bại nếu còn học kỳ hoặc tuần học liên kết.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá năm học thành công."));
    }
}
