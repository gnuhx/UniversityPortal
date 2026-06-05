/*
 * ChiTietCTDTController — Quản lý chi tiết chương trình đào tạo
 * ────────────────────────────────────────────────────────────────
 * Base route : /api/chi-tiet-ctdt
 * Phân quyền : Admin (CRUD), xem mở cho tất cả user đã đăng nhập
 *
 * Danh sách endpoint:
 *   GET    /api/chi-tiet-ctdt        — Danh sách (phân trang, lọc theo ctdtId)  [Authenticated]
 *   GET    /api/chi-tiet-ctdt/{id}   — Chi tiết một dòng                        [Authenticated]
 *   POST   /api/chi-tiet-ctdt        — Thêm môn vào CTDT                        [Admin]
 *   PUT    /api/chi-tiet-ctdt/{id}   — Cập nhật số tín chỉ / cờ tính điểm       [Admin]
 *   DELETE /api/chi-tiet-ctdt/{id}   — Xoá môn khỏi CTDT                       [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.ChiTietCTDT;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/chi-tiet-ctdt")]
[Authorize]
public class ChiTietCTDTController(IChiTietCTDTService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách chi tiết CTDT có phân trang.
    /// Truyền ctdtId để lọc theo một CTDT cụ thể (thường dùng khi xem danh sách môn của CTDT).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<ChiTietCTDTDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] int? ctdtId = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, ctdtId);
        return Ok(ApiResponseDto<PagedResultDto<ChiTietCTDTDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết một dòng theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ChiTietCTDTDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<ChiTietCTDTDto>.Ok(result));
    }

    /// <summary>
    /// Thêm môn học vào chương trình đào tạo.
    /// Kiểm tra trùng: một môn không được xuất hiện hai lần trong cùng CTDT.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<ChiTietCTDTDto>>> Create([FromBody] CreateChiTietCTDTDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<ChiTietCTDTDto>.Ok(result, "Thêm môn học vào CTDT thành công."));
    }

    /// <summary>
    /// Cập nhật số tín chỉ và cờ tính điểm trung bình của một dòng chi tiết.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<ChiTietCTDTDto>>> Update(int id, [FromBody] UpdateChiTietCTDTDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<ChiTietCTDTDto>.Ok(result, "Cập nhật chi tiết CTDT thành công."));
    }

    /// <summary>
    /// Xoá môn học khỏi chương trình đào tạo.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá chi tiết CTDT thành công."));
    }
}
