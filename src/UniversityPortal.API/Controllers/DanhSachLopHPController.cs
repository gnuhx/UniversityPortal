/*
 * DanhSachLopHPController — Danh sách lớp học phần & điểm
 * ─────────────────────────────────────────────────────────
 * Base route : /api/danh-sach-lop-hp
 *
 * Danh sách endpoint:
 *   GET /api/danh-sach-lop-hp/me          — Môn học + điểm của sinh viên đang đăng nhập [Sinh viên]
 *   GET /api/danh-sach-lop-hp?lopHpId=    — Danh sách sinh viên trong 1 lớp HP          [Admin, Giáo vụ, Giáo viên]
 */
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.DanhSachLopHP;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/danh-sach-lop-hp")]
[Authorize]
public class DanhSachLopHPController(IDanhSachLopHPService service) : ControllerBase
{
    /// <summary>
    /// Lấy toàn bộ môn học đã đăng ký và điểm số của sinh viên đang đăng nhập.
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<DanhSachLopHPDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetBySinhVienMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<DanhSachLopHPDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy danh sách sinh viên trong một lớp học phần.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ,Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<DanhSachLopHPDto>>>> GetByLopHocPhan(
        [FromQuery] int lopHpId)
    {
        var result = await service.GetByLopHocPhanAsync(lopHpId);
        return Ok(ApiResponseDto<IEnumerable<DanhSachLopHPDto>>.Ok(result));
    }
}
