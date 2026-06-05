/*
 * SinhVienController — Quản lý sinh viên
 * ────────────────────────────────────────
 * Base route : /api/sinh-vien
 * Phân quyền : Admin / GiaoVu (CRUD), SinhVien (xem bản thân)
 *
 * Danh sách endpoint:
 *   GET    /api/sinh-vien        — Danh sách (phân trang + lọc keyword, lopId) [Admin, GiaoVu]
 *   GET    /api/sinh-vien/{id}   — Chi tiết sinh viên                          [Admin, GiaoVu]
 *   POST   /api/sinh-vien        — Tạo mới sinh viên + tài khoản               [Admin]
 *   PUT    /api/sinh-vien/{id}   — Cập nhật thông tin                          [Admin, GiaoVu]
 *   DELETE /api/sinh-vien/{id}   — Khoá tài khoản (soft delete)               [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.SinhVien;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/sinh-vien")]
[Authorize]
public class SinhVienController(ISinhVienService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách sinh viên có phân trang.
    /// Lọc theo họ tên / MSSV (keyword) hoặc lớp sinh hoạt (lopId).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giao vu")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<SinhVienDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] int? lopId = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword, lopId);
        return Ok(ApiResponseDto<PagedResultDto<SinhVienDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết sinh viên theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Giao vu")]
    public async Task<ActionResult<ApiResponseDto<SinhVienDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<SinhVienDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới sinh viên kèm tài khoản đăng nhập với vai trò Sinh viên.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<SinhVienDto>>> Create([FromBody] CreateSinhVienDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<SinhVienDto>.Ok(result, "Tạo sinh viên thành công."));
    }

    /// <summary>
    /// Cập nhật thông tin sinh viên và tài khoản liên kết.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Giao vu")]
    public async Task<ActionResult<ApiResponseDto<SinhVienDto>>> Update(int id, [FromBody] UpdateSinhVienDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<SinhVienDto>.Ok(result, "Cập nhật sinh viên thành công."));
    }

    /// <summary>
    /// Khoá tài khoản sinh viên (soft delete — giữ lại lịch sử điểm và học phí).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Khoá tài khoản sinh viên thành công."));
    }
}
