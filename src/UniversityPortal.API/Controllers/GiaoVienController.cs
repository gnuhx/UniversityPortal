/*
 * GiaoVienController — Quản lý giáo viên
 * ──────────────────────────────────────
 * Base route : /api/giao-vien
 * Phân quyền : Admin (CRUD), GiaoVu (xem), GiaoVien (xem bản thân)
 *
 * Danh sách endpoint:
 *   GET    /api/giao-vien        — Danh sách giáo viên (phân trang + lọc) [Admin, GiaoVu]
 *   GET    /api/giao-vien/{id}   — Chi tiết giáo viên                     [Admin, GiaoVu]
 *   POST   /api/giao-vien        — Tạo mới giáo viên + tài khoản          [Admin]
 *   PUT    /api/giao-vien/{id}   — Cập nhật thông tin                     [Admin]
 *   DELETE /api/giao-vien/{id}   — Khoá tài khoản giáo viên (soft delete) [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.GiaoVien;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/giao-vien")]
[Authorize]
public class GiaoVienController(IGiaoVienService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách giáo viên có phân trang.
    /// Tìm kiếm theo họ tên hoặc mã GV.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<GiaoVienDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword);
        return Ok(ApiResponseDto<PagedResultDto<GiaoVienDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết giáo viên theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<GiaoVienDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<GiaoVienDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới giáo viên kèm tài khoản đăng nhập với vai trò Giáo viên.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<GiaoVienDto>>> Create([FromBody] CreateGiaoVienDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<GiaoVienDto>.Ok(result, "Tạo giáo viên thành công."));
    }

    /// <summary>
    /// Cập nhật thông tin giáo viên và tài khoản liên kết.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<GiaoVienDto>>> Update(int id, [FromBody] UpdateGiaoVienDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<GiaoVienDto>.Ok(result, "Cập nhật giáo viên thành công."));
    }

    /// <summary>
    /// Khoá tài khoản giáo viên (soft delete — không xoá dữ liệu lịch sử).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Khoá tài khoản giáo viên thành công."));
    }
}
