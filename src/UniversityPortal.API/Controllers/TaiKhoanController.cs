/*
 * TaiKhoanController — Quản lý tài khoản người dùng
 * ──────────────────────────────────────────────────
 * Base route : /api/tai-khoan
 * Phân quyền : Admin (CRUD toàn bộ), người dùng (đổi mật khẩu và ảnh đại diện của chính mình)
 *
 * Danh sách endpoint:
 *   GET    /api/tai-khoan                   — Lấy danh sách (phân trang + lọc) [Admin, GiaoVu]
 *   GET    /api/tai-khoan/{id}              — Lấy chi tiết theo id             [Admin, GiaoVu]
 *   POST   /api/tai-khoan                   — Tạo mới tài khoản                [Admin]
 *   PUT    /api/tai-khoan/{id}              — Cập nhật thông tin               [Admin]
 *   PATCH  /api/tai-khoan/{id}/trang-thai   — Khoá / mở tài khoản             [Admin]
 *   PUT    /api/tai-khoan/{id}/doi-mat-khau — Đổi mật khẩu (bản thân)         [Authenticated]
 *   POST   /api/tai-khoan/{id}/avatar       — Upload ảnh đại diện              [Authenticated]
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.TaiKhoan;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.API.Controllers;

[ApiController]
[Route("api/tai-khoan")]
[Authorize]
public class TaiKhoanController(ITaiKhoanService service) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách tài khoản có phân trang và bộ lọc.
    /// Chỉ Admin và GiaoVu được truy cập.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<TaiKhoanDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] int? vaiTroId = null,
        [FromQuery] bool? trangThai = null)
    {
        var result = await service.GetPagedAsync(page, pageSize, keyword, vaiTroId, trangThai);
        return Ok(ApiResponseDto<PagedResultDto<TaiKhoanDto>>.Ok(result));
    }

    /// <summary>
    /// Lấy thông tin chi tiết một tài khoản theo id.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Giáo vụ")]
    public async Task<ActionResult<ApiResponseDto<TaiKhoanDto>>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return Ok(ApiResponseDto<TaiKhoanDto>.Ok(result));
    }

    /// <summary>
    /// Tạo mới tài khoản. FluentValidation tự động xác thực request body.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<TaiKhoanDto>>> Create([FromBody] CreateTaiKhoanDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponseDto<TaiKhoanDto>.Ok(result, "Tạo tài khoản thành công."));
    }

    /// <summary>
    /// Cập nhật thông tin tài khoản theo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<TaiKhoanDto>>> Update(int id, [FromBody] UpdateTaiKhoanDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return Ok(ApiResponseDto<TaiKhoanDto>.Ok(result, "Cập nhật tài khoản thành công."));
    }

    /// <summary>
    /// Bật/tắt trạng thái khoá của tài khoản (Admin only).
    /// </summary>
    [HttpPatch("{id:int}/trang-thai")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<TaiKhoanDto>>> ToggleTrangThai(int id)
    {
        var result = await service.ToggleTrangThaiAsync(id);
        return Ok(ApiResponseDto<TaiKhoanDto>.Ok(result, "Cập nhật trạng thái thành công."));
    }

    /// <summary>
    /// Người dùng tự đổi mật khẩu của chính mình.
    /// Yêu cầu nhập mật khẩu cũ để xác thực quyền sở hữu.
    /// </summary>
    [HttpPut("{id:int}/doi-mat-khau")]
    public async Task<ActionResult<ApiResponseDto<object>>> DoiMatKhau(int id, [FromBody] DoiMatKhauDto dto)
    {
        await service.DoiMatKhauAsync(id, dto);
        return Ok(ApiResponseDto<object>.Ok(null, "Đổi mật khẩu thành công."));
    }

    /// <summary>
    /// Upload ảnh đại diện cho tài khoản.
    /// Chỉ chấp nhận file ảnh (jpg, jpeg, png, webp) tối đa 2MB.
    /// </summary>
    [HttpPost("{id:int}/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponseDto<object>>> UploadAvatar(int id, IFormFile file)
    {
        // Kiểm tra định dạng và kích thước file ảnh trước khi xử lý
        var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExt.Contains(ext))
            return BadRequest(ApiResponseDto<object>.Fail("Chỉ chấp nhận file ảnh jpg, jpeg, png, webp."));

        if (file.Length > 2 * 1024 * 1024)
            return BadRequest(ApiResponseDto<object>.Fail("Kích thước ảnh không quá 2MB."));

        var url = await service.CapNhatAnhDaiDienAsync(id, file);
        return Ok(ApiResponseDto<object>.Ok(new { url }, "Upload ảnh đại diện thành công."));
    }
}
