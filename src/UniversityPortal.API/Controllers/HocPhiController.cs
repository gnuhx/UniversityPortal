using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.HocPhi;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/hoc-phi")]
[Authorize]
public class HocPhiController(IHocPhiService service) : ControllerBase
{
    /// <summary>Sinh viên xem học phí của mình theo học kỳ.</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Sinh viên")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<HocPhiDto>>>> GetMe()
    {
        var taiKhoanId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.GetByMeAsync(taiKhoanId);
        return Ok(ApiResponseDto<IEnumerable<HocPhiDto>>.Ok(result));
    }

    /// <summary>Admin xem học phí theo học kỳ.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<HocPhiDto>>>> GetByHocKy([FromQuery] int hocKyId)
    {
        var result = await service.GetByHocKyAsync(hocKyId);
        return Ok(ApiResponseDto<IEnumerable<HocPhiDto>>.Ok(result));
    }

    /// <summary>Admin tạo học phí cho sinh viên.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<HocPhiDto>>> Create([FromBody] CreateHocPhiDto dto)
    {
        var result = await service.CreateAsync(dto);
        return Ok(ApiResponseDto<HocPhiDto>.Ok(result, "Tạo học phí thành công."));
    }

    /// <summary>Admin cập nhật trạng thái đóng tiền.</summary>
    [HttpPut("{id:int}/trang-thai")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<HocPhiDto>>> UpdateTrangThai(int id, [FromBody] UpdateTrangThaiDto dto)
    {
        var result = await service.UpdateTrangThaiAsync(id, dto.TrangThai);
        return Ok(ApiResponseDto<HocPhiDto>.Ok(result));
    }
}
