// Dịch vụ quản lý giáo viên — tạo kèm tài khoản, cập nhật, xoá mềm
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.GiaoVien;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý giáo viên.
/// Mỗi thao tác tạo/xoá giáo viên đồng thời tác động lên bảng TaiKhoan liên kết.
/// </summary>
public class GiaoVienService(IUnitOfWork uow, IMapper mapper) : IGiaoVienService
{
    /// <summary>Lấy danh sách giáo viên có phân trang và lọc theo keyword.</summary>
    public async Task<PagedResultDto<GiaoVienDto>> GetPagedAsync(int page, int pageSize, string? keyword)
    {
        var paged = await uow.GiaoViens.GetPagedFilterAsync(page, pageSize, keyword);
        return new PagedResultDto<GiaoVienDto>
        {
            Data     = mapper.Map<IEnumerable<GiaoVienDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy chi tiết giáo viên theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<GiaoVienDto> GetByIdAsync(int id)
    {
        var gv = await uow.GiaoViens.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy giáo viên id = {id}.");
        return mapper.Map<GiaoVienDto>(gv);
    }

    /// <summary>
    /// Tạo mới giáo viên kèm tài khoản đăng nhập với vai trò Giáo viên (VaiTroId = 2).
    /// Ném BadRequestException nếu tên đăng nhập hoặc mã GV đã tồn tại.
    /// </summary>
    public async Task<GiaoVienDto> CreateAsync(CreateGiaoVienDto dto)
    {
        if (await uow.TaiKhoans.GetByTenDangNhapAsync(dto.TenDangNhap) is not null)
            throw new BadRequestException($"Tên đăng nhập '{dto.TenDangNhap}' đã tồn tại.");

        if (await uow.GiaoViens.GetByMaGvAsync(dto.MaGv) is not null)
            throw new BadRequestException($"Mã giáo viên '{dto.MaGv}' đã tồn tại.");

        // Tạo tài khoản trước, sau đó liên kết GiaoVien — đảm bảo foreign key hợp lệ
        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = dto.TenDangNhap,
            MatKhau     = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
            VaiTroId    = 2, // Giáo viên
            PhongBanId  = dto.PhongBanId,
            HoTen       = dto.HoTen,
            Email       = dto.Email,
            TrangThai   = true
        };
        await uow.TaiKhoans.AddAsync(taiKhoan);
        await uow.CommitAsync();

        var giaoVien = new GiaoVien
        {
            TaiKhoanId = taiKhoan.Id,
            MaGv       = dto.MaGv
        };
        await uow.GiaoViens.AddAsync(giaoVien);
        await uow.CommitAsync();

        return mapper.Map<GiaoVienDto>(await uow.GiaoViens.GetDetailAsync(giaoVien.Id));
    }

    /// <summary>Cập nhật thông tin giáo viên và tài khoản liên kết.</summary>
    public async Task<GiaoVienDto> UpdateAsync(int id, UpdateGiaoVienDto dto)
    {
        var gv = await uow.GiaoViens.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy giáo viên id = {id}.");

        // Kiểm tra mã GV mới không trùng với giáo viên khác
        var existing = await uow.GiaoViens.GetByMaGvAsync(dto.MaGv);
        if (existing is not null && existing.Id != id)
            throw new BadRequestException($"Mã giáo viên '{dto.MaGv}' đã được sử dụng.");

        gv.MaGv              = dto.MaGv;
        gv.TaiKhoan.HoTen    = dto.HoTen;
        gv.TaiKhoan.Email    = dto.Email;
        gv.TaiKhoan.PhongBanId = dto.PhongBanId;
        gv.TaiKhoan.TrangThai  = dto.TrangThai;

        uow.GiaoViens.Update(gv);
        await uow.CommitAsync();

        return mapper.Map<GiaoVienDto>(gv);
    }

    /// <summary>
    /// Xoá mềm giáo viên bằng cách khoá tài khoản liên kết (TrangThai = false).
    /// Không xoá bản ghi để giữ lại lịch sử điểm, lớp học phần.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var gv = await uow.GiaoViens.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy giáo viên id = {id}.");

        gv.TaiKhoan.TrangThai = false;
        uow.GiaoViens.Update(gv);
        await uow.CommitAsync();
    }
}
