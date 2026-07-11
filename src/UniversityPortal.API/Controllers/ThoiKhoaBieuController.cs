/*
 * ThoiKhoaBieuController — Quản lý & tra cứu thời khoá biểu
 * ────────────────────────────────────────────────────────────
 * Base route : /api/thoi-khoa-bieu
 * Phân quyền : Admin/Giáo vụ (CRUD), Sinh viên/Giáo viên (xem lịch của mình)
 *
 * Danh sách endpoint:
 *   GET    /api/thoi-khoa-bieu/me           — Lịch của tôi (Sinh viên/Giáo viên, lọc theo học kỳ) [Sinh viên, Giáo viên]
 *   GET    /api/thoi-khoa-bieu               — Danh sách buổi học (phân trang, lọc lớp HP)         [Admin, Giáo vụ]
 *   GET    /api/thoi-khoa-bieu/{id}          — Chi tiết một buổi học                                [Admin, Giáo vụ]
 *   POST   /api/thoi-khoa-bieu               — Thêm buổi học vào lớp HP                             [Admin, Giáo vụ]
 *   POST   /api/thoi-khoa-bieu/generate      — Tạo hàng loạt buổi học lặp lại theo tuần             [Admin, Giáo vụ]
 *   PUT    /api/thoi-khoa-bieu/{id}          — Cập nhật buổi học                                    [Admin, Giáo vụ]
 *   DELETE /api/thoi-khoa-bieu/{id}          — Xoá buổi học                                         [Admin, Giáo vụ]
 */
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ThoiKhoaBieu;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/thoi-khoa-bieu")]
[Authorize]
public class ThoiKhoaBieuController(IThoiKhoaBieuService service) : ControllerBase
{
    /// <summary>
    /// Lấy lịch học/lịch dạy của người dùng đang đăng nhập, lọc theo học kỳ nếu truyền hocKyId.
    /// Tự phân nhánh theo vai trò: Sinh viên xem lớp đã đăng ký, Giáo viên xem lớp mình dạy.
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Sinh viên,Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ThoiKhoaBieuDto>>>> GetMe([FromQuery] int? hocKyId)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = User.IsInRole("Sinh viên")
            ? await service.GetForSinhVienMeAsync(taiKhoanId, hocKyId)
            : await service.GetForGiaoVienMeAsync(taiKhoanId, hocKyId);
        return Ok(ApiResponseDto<IEnumerable<ThoiKhoaBieuDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy danh sách buổi học có phân trang, lọc theo lớp học phần (dùng khi quản lý TKB của một lớp).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<ThoiKhoaBieuDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] int? lopHpId = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, lopHpId);
        return Ok(ApiResponseDto<PagedResultDto<ThoiKhoaBieuDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy chi tiết một buổi học theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<ThoiKhoaBieuDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<ThoiKhoaBieuDto>.Ok(result));
    }

    /// <summary>
    /// Thêm một buổi học vào lớp học phần. Kiểm tra trùng phòng/giờ trong cùng tuần + thứ.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<ThoiKhoaBieuDto>>> Create([FromBody] CreateThoiKhoaBieuDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<ThoiKhoaBieuDto>.Ok(result, "Thêm buổi học thành công."));
    }

    /// <summary>
    /// Tạo hàng loạt buổi học lặp lại hàng tuần (cùng thứ/tiết/phòng) cho một lớp HP,
    /// từ tuần bắt đầu đến tuần kết thúc. Tuần nào trùng phòng/giờ với buổi học khác sẽ bị bỏ qua.
    /// </summary>
    [HttpPost("generate")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<GenerateThoiKhoaBieuResultDto>>> Generate([FromBody] GenerateThoiKhoaBieuDto dto)
    {
        var result = await service.GenerateAsync(dto);
        var message = result.TuanBiBoQua.Count == 0
            ? $"Đã tạo {result.SoBuoiDaTao} buổi học."
            : $"Đã tạo {result.SoBuoiDaTao} buổi học. Bỏ qua {result.TuanBiBoQua.Count} tuần do trùng phòng/giờ: {string.Join(", ", result.TuanBiBoQua)}.";
        return Ok(ApiResponseDto<GenerateThoiKhoaBieuResultDto>.Ok(result, message));
    }

    /// <summary>
    /// Cập nhật tuần/thứ/tiết/phòng của một buổi học.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<ThoiKhoaBieuDto>>> Update(int id, [FromBody] UpdateThoiKhoaBieuDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<ThoiKhoaBieuDto>.Ok(result, "Cập nhật buổi học thành công."));
    }

    /// <summary>
    /// Xoá một buổi học khỏi thời khoá biểu.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Xoá buổi học thành công."));
    }
}
