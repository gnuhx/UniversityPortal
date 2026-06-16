/*
 * LopSinhHoatController — Quản lý lớp sinh hoạt
 * ─────────────────────────────────────────────────
 * Base route : /api/lop-sinh-hoat
 * Phân quyền : Admin / GiaoVu (CRUD), GiaoVien (xem lớp mình phụ trách), SinhVien (xem lớp mình)
 *
 * Danh sách endpoint:
 *   GET    /api/lop-sinh-hoat        — Danh sách (phân trang + lọc)  [Admin, GiaoVu]
 *   GET    /api/lop-sinh-hoat/all    — Tất cả lớp (dropdown)         [Authenticated]
 *   GET    /api/lop-sinh-hoat/{id}   — Chi tiết lớp sinh hoạt        [Authenticated]
 *   POST   /api/lop-sinh-hoat        — Tạo mới lớp (ThuKyId = null)  [Admin, GiaoVu]
 *   PUT    /api/lop-sinh-hoat/{id}   — Cập nhật lớp (gán thư ký)    [Admin, GiaoVu]
 *   DELETE /api/lop-sinh-hoat/{id}   — Xoá lớp (phải trống SV)      [Admin]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.LopSinhHoat;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/lop-sinh-hoat")]
[Authorize]
public class LopSinhHoatController(ILopSinhHoatService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách lớp sinh hoạt có phân trang.
    /// Lọc theo mã lớp (keyword) hoặc GVCN (gvcnId).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<LopSinhHoatDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] int? gvcnId = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword, gvcnId);
        return Ok(ApiResponseDto<PagedResultDto<LopSinhHoatDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy tất cả lớp sinh hoạt (dùng cho dropdown chọn lớp khi tạo sinh viên).
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<LopSinhHoatDto>>>> GetAll()
    {
        var result = await service.GetAllAsync();
        return Ok(ApiResponseDto<IEnumerable<LopSinhHoatDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết lớp sinh hoạt theo id, bao gồm GVCN, thư ký và số sinh viên.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<LopSinhHoatDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<LopSinhHoatDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới lớp sinh hoạt.
    /// ThuKyId luôn = null khi tạo — gán thư ký sau qua PUT khi sinh viên đã có lớp.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<LopSinhHoatDto>>> Create([FromBody] CreateLopSinhHoatDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<LopSinhHoatDto>.Ok(result, "Tạo lớp sinh hoạt thành công."));
    }

    /// <summary>
    /// Cập nhật lớp sinh hoạt.
    /// Dùng endpoint này để gán thư ký sau khi sinh viên đã được thêm vào lớp.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<LopSinhHoatDto>>> Update(int id, [FromBody] UpdateLopSinhHoatDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<LopSinhHoatDto>.Ok(result, "Cập nhật lớp sinh hoạt thành công."));
    }

    /// <summary>
    /// Xoá lớp sinh hoạt. Sẽ thất bại nếu còn sinh viên trong lớp.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá lớp sinh hoạt thành công."));
    }
}
