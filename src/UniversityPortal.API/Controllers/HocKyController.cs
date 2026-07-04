using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/hoc-ky")]
[Authorize]
public class HocKyController(IUnitOfWork uow) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<object>>> GetAll()
    {
        var list = await uow.HocKys.GetAllAsync();
        var result = list.Select(x => new
        {
            x.Id,
            x.TenHocKy,
            x.NgayBatDau,
            TenNamHoc = x.NamHoc?.TenNamHoc ?? string.Empty,
        });
        return Ok(ApiResponseDto<object>.Ok(result));
    }
}
