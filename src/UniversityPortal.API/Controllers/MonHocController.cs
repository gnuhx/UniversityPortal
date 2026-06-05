/*
 * MonHocController — Quản lý môn học
 * ────────────────────────────────────
 * Base route : /api/mon-hoc
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/mon-hoc        — Danh sách (phân trang + lọc)  [Authenticated]
 *   GET    /api/mon-hoc/all    — Tất cả môn học (dropdown)     [Authenticated]
 *   GET    /api/mon-hoc/{id}   — Chi tiết môn học              [Authenticated]
 *   POST   /api/mon-hoc        — Tạo mới môn học               [Admin]
 *   PUT    /api/mon-hoc/{id}   — Cập nhật môn học              [Admin]
 *   DELETE /api/mon-hoc/{id}   — Xoá môn học                  [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.MonHoc;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/mon-hoc")]
[Authorize]
public class MonHocController(IMonHocService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách môn học có phân trang, lọc theo mã hoặc tên môn.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<MonHocDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword);
        return Ok(ApiResponseDto<PagedResultDto<MonHocDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy tất cả môn học (dùng cho dropdown chọn môn trong chi tiết CTDT).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<MonHocDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<MonHocDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết môn học theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<MonHocDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<MonHocDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới môn học.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<MonHocDto>>> Create([FromBody] UpsertMonHocDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<MonHocDto>.Ok(result, "Tạo môn học thành công."));
    }

    /// <summary>
    /// Cập nhật môn học theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<MonHocDto>>> Update(int id, [FromBody] UpsertMonHocDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<MonHocDto>.Ok(result, "Cập nhật môn học thành công."));
    }

    /// <summary>
    /// Xoá môn học. Sẽ thất bại nếu còn trong chi tiết CTDT.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá môn học thành công."));
    }
}
