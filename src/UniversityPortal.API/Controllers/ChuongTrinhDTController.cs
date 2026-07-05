/*
 * ChuongTrinhDTController — Quản lý chương trình đào tạo
 * ────────────────────────────────────────────────────────
 * Base route : /api/chuong-trinh-dt
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/chuong-trinh-dt        — Danh sách (phân trang + lọc ngành)  [Authenticated]
 *   GET    /api/chuong-trinh-dt/all    — Tất cả CTDT (dùng dropdown)         [Authenticated]
 *   GET    /api/chuong-trinh-dt/{id}   — Chi tiết CTDT                       [Authenticated]
 *   POST   /api/chuong-trinh-dt        — Tạo mới CTDT                        [Admin]
 *   PUT    /api/chuong-trinh-dt/{id}   — Cập nhật CTDT                       [Admin]
 *   DELETE /api/chuong-trinh-dt/{id}   — Xoá CTDT                           [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ChuongTrinhDT;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/chuong-trinh-dt")]
[Authorize]
public class ChuongTrinhDTController(IChuongTrinhDTService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách CTDT có phân trang, lọc theo keyword, ngành và khoá học (exact match).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<ChuongTrinhDTDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] int? nganhId = null,
        [FromQuery] string? khoaHoc = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword, nganhId, khoaHoc);
        return Ok(ApiResponseDto<PagedResultDto<ChuongTrinhDTDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy tất cả CTDT không phân trang (dùng cho dropdown chọn lớp sinh hoạt).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ChuongTrinhDTDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<ChuongTrinhDTDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết CTDT theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ChuongTrinhDTDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<ChuongTrinhDTDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới CTDT.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<ChuongTrinhDTDto>>> Create([FromBody] UpsertChuongTrinhDTDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<ChuongTrinhDTDto>.Ok(result, "Tạo chương trình đào tạo thành công."));
    }

    /// <summary>
    /// Cập nhật CTDT theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<ChuongTrinhDTDto>>> Update(int id, [FromBody] UpsertChuongTrinhDTDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<ChuongTrinhDTDto>.Ok(result, "Cập nhật chương trình đào tạo thành công."));
    }

    /// <summary>
    /// Xoá CTDT. Sẽ thất bại nếu còn chi tiết môn học liên kết.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá chương trình đào tạo thành công."));
    }
}
