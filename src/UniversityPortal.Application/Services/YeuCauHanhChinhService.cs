using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.YeuCauHanhChinh;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class YeuCauHanhChinhService(IUnitOfWork uow) : IYeuCauHanhChinhService
{
    public async Task<IEnumerable<YeuCauHanhChinhDto>> GetByMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        var list = await uow.YeuCauHanhChinhs.GetBySinhVienAsync(sv.Id);
        return list.Select(MapToDto);
    }

    public async Task<PagedResultDto<YeuCauHanhChinhDto>> GetAllAsync(int page, int pageSize, string? trangThai)
    {
        var paged = await uow.YeuCauHanhChinhs.GetAllPagedAsync(page, pageSize, trangThai);
        return new PagedResultDto<YeuCauHanhChinhDto>
        {
            Data     = paged.Data.Select(MapToDto),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize,
        };
    }

    public async Task<YeuCauHanhChinhDto> CreateAsync(int taiKhoanId, CreateYeuCauHanhChinhDto dto)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        if (dto.LoaiYeuCau == "Giấy Xác Nhận" && string.IsNullOrWhiteSpace(dto.LoaiGiayXacNhan))
            throw new BadRequestException("Vui lòng chọn loại giấy xác nhận.");

        var yc = new YeuCauHanhChinh
        {
            SinhVienId       = sv.Id,
            LoaiYeuCau       = dto.LoaiYeuCau,
            LoaiGiayXacNhan  = dto.LoaiYeuCau == "Giấy Xác Nhận" ? dto.LoaiGiayXacNhan : null,
            NoiDung          = dto.NoiDung,
            FileDinhKem      = dto.FileDinhKem,
            TrangThai        = "Chờ duyệt",
            NgayTao          = DateTime.UtcNow,
        };

        await uow.YeuCauHanhChinhs.AddAsync(yc);
        await uow.CommitAsync();

        var saved = await uow.YeuCauHanhChinhs.GetDetailAsync(yc.Id)
            ?? throw new NotFoundException("Lỗi khi lưu yêu cầu.");

        return MapToDto(saved);
    }

    public async Task<YeuCauHanhChinhDto> DuyetAsync(int id, int taiKhoanId, DuyetYeuCauHanhChinhDto dto)
    {
        var allowed = new[] { "Đã duyệt", "Từ chối" };
        if (!allowed.Contains(dto.TrangThai))
            throw new BadRequestException($"Trạng thái không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowed)}.");

        var yc = await uow.YeuCauHanhChinhs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy yêu cầu ID = {id}.");

        if (yc.TrangThai != "Chờ duyệt")
            throw new BadRequestException("Yêu cầu này đã được xử lý rồi.");

        yc.TrangThai    = dto.TrangThai;
        yc.NguoiDuyetId = taiKhoanId;
        yc.GhiChuAdmin  = dto.GhiChu;

        await uow.CommitAsync();

        var updated = await uow.YeuCauHanhChinhs.GetDetailAsync(id)!;
        return MapToDto(updated!);
    }

    private static YeuCauHanhChinhDto MapToDto(YeuCauHanhChinh yc) => new()
    {
        Id             = yc.Id,
        SinhVienId     = yc.SinhVienId,
        TenSinhVien    = yc.SinhVien?.TaiKhoan?.HoTen ?? string.Empty,
        Mssv           = yc.SinhVien?.Mssv ?? string.Empty,
        LoaiYeuCau       = yc.LoaiYeuCau,
        LoaiGiayXacNhan  = yc.LoaiGiayXacNhan,
        NoiDung          = yc.NoiDung,
        FileDinhKem      = yc.FileDinhKem,
        TrangThai        = yc.TrangThai,
        NguoiDuyetId     = yc.NguoiDuyetId,
        TenNguoiDuyet    = yc.NguoiDuyet?.HoTen,
        GhiChuAdmin      = yc.GhiChuAdmin,
        NgayTao          = yc.NgayTao,
        CreatedAt        = yc.CreatedAt,
    };
}
