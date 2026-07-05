using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.LopHocPhan;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/lop-hoc-phan")]
[Authorize]
public class LopHocPhanController(ILopHocPhanService service) : ControllerBase
{
    /// <summary>Giáo viên xem tất cả lớp HP mình đang phụ trách.</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<LopHocPhanDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetByGiaoVienMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<LopHocPhanDto>>.Ok(result));
    }

    /// <summary>Admin/Giáo vụ duyệt danh sách lớp học phần có phân trang, lọc theo học kỳ và/hoặc mã lớp.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<LopHocPhanDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? hocKyId = null,
        [FromQuery] string? keyword = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, hocKyId, keyword);
        return Ok(ApiResponseDto<PagedResultDto<LopHocPhanDto>>.Ok(result));
    }

    /// <summary>Giáo viên khoá bảng điểm.</summary>
    [HttpPut("{id:int}/khoa-bang-diem")]
    [Authorize(Roles = "Giáo viên")]
    public async Task<ActionResult<ApiResponseDto<object>>> KhoaBangDiem(int id)
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await service.KhoaBangDiemAsync(id, taiKhoanId);
        return Ok(ApiResponseDto<object>.Ok(null, "Đã khoá bảng điểm thành công."));
    }

    /// <summary>Admin mở khoá bảng điểm.</summary>
    [HttpPut("{id:int}/mo-bang-diem")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<object>>> MoBangDiem(int id)
    {
        await service.MoBangDiemAsync(id);
        return Ok(ApiResponseDto<object>.Ok(null, "Đã mở khoá bảng điểm thành công."));
    }
}
