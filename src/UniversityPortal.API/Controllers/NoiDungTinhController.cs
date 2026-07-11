/*
 * NoiDungTinhController — Quản lý nội dung tĩnh theo khu vực (Thư viện, Học Vụ,...)
 * ────────────────────────────────────────────────────────────────────────────
 * Base route : /api/noi-dung-tinh
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/noi-dung-tinh?khuVuc=thu-vien  — Danh sách theo khu vực, sắp theo Thứ tự  [Authenticated]
 *   GET    /api/noi-dung-tinh/{id}             — Chi tiết 1 mục                           [Authenticated]
 *   POST   /api/noi-dung-tinh                  — Tạo mới mục nội dung                     [Admin]
 *   PUT    /api/noi-dung-tinh/{id}              — Cập nhật mục nội dung                    [Admin]
 *   DELETE /api/noi-dung-tinh/{id}              — Xoá mục nội dung                        [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NoiDungTinh;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/noi-dung-tinh")]
[Authorize]
public class NoiDungTinhController(INoiDungTinhService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách mục nội dung của 1 khu vực (vd "thu-vien", "hoc-vu"), sắp theo Thứ tự.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<NoiDungTinhDto>>>> GetByKhuVuc([FromQuery] string khuVuc)
    {
        var result = await service.GetByKhuVucAsync(khuVuc);
        return Ok(ApiResponseDto<IEnumerable<NoiDungTinhDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết 1 mục nội dung theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<NoiDungTinhDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<NoiDungTinhDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới mục nội dung.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<NoiDungTinhDto>>> Create([FromBody] UpsertNoiDungTinhDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<NoiDungTinhDto>.Ok(result, "Tạo mục nội dung thành công."));
    }

    /// <summary>
    /// Cập nhật mục nội dung theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<NoiDungTinhDto>>> Update(int id, [FromBody] UpsertNoiDungTinhDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<NoiDungTinhDto>.Ok(result, "Cập nhật mục nội dung thành công."));
    }

    /// <summary>
    /// Xoá mục nội dung theo id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá mục nội dung thành công."));
    }
}
