// Dịch vụ quản lý tài khoản — CRUD, đổi mật khẩu, khoá/mở, upload ảnh đại diện
using AutoMapper;
using Microsoft.AspNetCore.Http;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.TaiKhoan;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý tài khoản người dùng.
/// Sử dụng UnitOfWork để đảm bảo tính nhất quán dữ liệu.
/// </summary>
public class TaiKhoanService(
    IUnitOfWork uow,
    IMapper mapper,
    IFileService fileService) : ITaiKhoanService
{
    /// <summary>Lấy danh sách tài khoản có phân trang và bộ lọc theo keyword, vai trò, trạng thái.</summary>
    public async Task<PagedResultDto<TaiKhoanDto>> GetPagedAsync(
        int page, int pageSize, string? keyword, int? vaiTroId, bool? trangThai)
    {
        var paged = await uow.TaiKhoans.GetPagedFilterAsync(page, pageSize, keyword, vaiTroId, trangThai);
        return new PagedResultDto<TaiKhoanDto>
        {
            Data     = mapper.Map<IEnumerable<TaiKhoanDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy thông tin chi tiết tài khoản theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<TaiKhoanDto> GetByIdAsync(int id)
    {
        var tk = await uow.TaiKhoans.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy tài khoản id = {id}.");
        return mapper.Map<TaiKhoanDto>(tk);
    }

    /// <summary>
    /// Tạo mới tài khoản.
    /// Ném BadRequestException nếu tên đăng nhập hoặc email đã tồn tại.
    /// Mật khẩu được hash bằng BCrypt trước khi lưu.
    /// </summary>
    public async Task<TaiKhoanDto> CreateAsync(CreateTaiKhoanDto dto)
    {
        // Kiểm tra tên đăng nhập chưa tồn tại trong hệ thống
        if (await uow.TaiKhoans.GetByTenDangNhapAsync(dto.TenDangNhap) is not null)
            throw new BadRequestException($"Tên đăng nhập '{dto.TenDangNhap}' đã tồn tại.");

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = dto.TenDangNhap,
            MatKhau     = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
            VaiTroId    = dto.VaiTroId,
            PhongBanId  = dto.PhongBanId,
            HoTen       = dto.HoTen,
            Email       = dto.Email,
            TrangThai   = true
        };

        await uow.TaiKhoans.AddAsync(taiKhoan);
        await uow.CommitAsync();

        // Load lại navigation properties cho mapper
        return mapper.Map<TaiKhoanDto>(await uow.TaiKhoans.GetByIdAsync(taiKhoan.Id));
    }

    /// <summary>Cập nhật thông tin tài khoản. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<TaiKhoanDto> UpdateAsync(int id, UpdateTaiKhoanDto dto)
    {
        var taiKhoan = await uow.TaiKhoans.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy tài khoản id = {id}.");

        taiKhoan.VaiTroId   = dto.VaiTroId;
        taiKhoan.PhongBanId = dto.PhongBanId;
        taiKhoan.HoTen      = dto.HoTen;
        taiKhoan.Email      = dto.Email;
        taiKhoan.TrangThai  = dto.TrangThai;

        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();

        return mapper.Map<TaiKhoanDto>(taiKhoan);
    }

    /// <summary>
    /// Đổi mật khẩu tài khoản.
    /// Xác thực mật khẩu cũ trước; ném BadRequestException nếu không khớp hoặc mật khẩu mới != xác nhận.
    /// </summary>
    public async Task DoiMatKhauAsync(int id, DoiMatKhauDto dto)
    {
        var taiKhoan = await uow.TaiKhoans.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy tài khoản id = {id}.");

        if (!BCrypt.Net.BCrypt.Verify(dto.MatKhauCu, taiKhoan.MatKhau))
            throw new BadRequestException("Mật khẩu cũ không đúng.");

        if (dto.MatKhauMoi != dto.XacNhanMatKhau)
            throw new BadRequestException("Mật khẩu mới và xác nhận mật khẩu không khớp.");

        taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhauMoi);
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();
    }

    /// <summary>Đảo ngược trạng thái hoạt động (khoá ↔ mở) của tài khoản.</summary>
    public async Task<TaiKhoanDto> ToggleTrangThaiAsync(int id)
    {
        var taiKhoan = await uow.TaiKhoans.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy tài khoản id = {id}.");

        taiKhoan.TrangThai = !taiKhoan.TrangThai;
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();

        return mapper.Map<TaiKhoanDto>(taiKhoan);
    }

    /// <summary>
    /// Cập nhật ảnh đại diện.
    /// Xoá ảnh cũ trên disk (nếu có) trước khi lưu ảnh mới, tránh tốn dung lượng.
    /// </summary>
    public async Task<string> CapNhatAnhDaiDienAsync(int id, IFormFile file)
    {
        var taiKhoan = await uow.TaiKhoans.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy tài khoản id = {id}.");

        // Xoá ảnh cũ trước khi lưu ảnh mới để tránh tích luỹ file rác
        fileService.DeleteFile(taiKhoan.AnhDaiDien);

        var duongDan = await fileService.SaveAvatarAsync(file, id);
        taiKhoan.AnhDaiDien = duongDan;
        uow.TaiKhoans.Update(taiKhoan);
        await uow.CommitAsync();

        return duongDan;
    }
}
