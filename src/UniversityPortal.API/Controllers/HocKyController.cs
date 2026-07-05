/*
 * HocKyController — Quản lý học kỳ
 * ─────────────────────────────────────────
 * Base route : /api/hoc-ky
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/hoc-ky        — Danh sách (phân trang, lọc theo năm học)  [Authenticated]
 *   GET    /api/hoc-ky/all    — Tất cả học kỳ (dùng dropdown)             [Authenticated]
 *   GET    /api/hoc-ky/{id}   — Chi tiết học kỳ                          [Authenticated]
 *   POST   /api/hoc-ky        — Tạo mới học kỳ                           [Admin]
 *   PUT    /api/hoc-ky/{id}   — Cập nhật học kỳ                          [Admin]
 *   DELETE /api/hoc-ky/{id}   — Xoá học kỳ                              [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.HocKy;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/hoc-ky")]
[Authorize]
public class HocKyController(IHocKyService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách học kỳ có phân trang, lọc theo năm học — dùng cho trang quản lý học kỳ
    /// của một năm học cụ thể.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<HocKyDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? namHocId = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, namHocId);
        return Ok(ApiResponseDto<PagedResultDto<HocKyDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy tất cả học kỳ không phân trang, sắp giảm dần theo ngày bắt đầu
    /// (dùng cho dropdown chọn học kỳ ở các trang khác).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<HocKyDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<HocKyDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết học kỳ theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<HocKyDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<HocKyDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới học kỳ.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<HocKyDto>>> Create([FromBody] UpsertHocKyDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<HocKyDto>.Ok(result, "Tạo học kỳ thành công."));
    }

    /// <summary>
    /// Cập nhật học kỳ theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<HocKyDto>>> Update(int id, [FromBody] UpsertHocKyDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<HocKyDto>.Ok(result, "Cập nhật học kỳ thành công."));
    }

    /// <summary>
    /// Xoá học kỳ. Sẽ thất bại nếu còn lớp học phần, chi tiết CTĐT hoặc học phí liên kết.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá học kỳ thành công."));
    }
}
