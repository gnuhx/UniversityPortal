// Dịch vụ quản lý chương trình đào tạo
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ChuongTrinhDT;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý chương trình đào tạo (CTDT).
/// </summary>
public class ChuongTrinhDTService(IUnitOfWork uow, IMapper mapper) : IChuongTrinhDTService
{
    /// <summary>Lấy danh sách CTDT có phân trang và lọc.</summary>
    public async Task<PagedResultDto<ChuongTrinhDTDto>> GetPagedAsync(
        int page, int pageSize, string? keyword, int? nganhId, string? khoaHoc)
    {
        var paged = await uow.ChuongTrinhDTs.GetPagedFilterAsync(page, pageSize, keyword, nganhId, khoaHoc);
        return new PagedResultDto<ChuongTrinhDTDto>
        {
            Data     = mapper.Map<IEnumerable<ChuongTrinhDTDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy tất cả CTDT (không phân trang, dùng cho dropdown).</summary>
    public async Task<IEnumerable<ChuongTrinhDTDto>> GetAllAsync()
        => mapper.Map<IEnumerable<ChuongTrinhDTDto>>(await uow.ChuongTrinhDTs.GetAllAsync());

    /// <summary>Lấy chi tiết CTDT theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<ChuongTrinhDTDto> GetByIdAsync(int id)
    {
        var ctdt = await uow.ChuongTrinhDTs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy chương trình đào tạo id = {id}.");
        return mapper.Map<ChuongTrinhDTDto>(ctdt);
    }

    /// <summary>Tạo mới CTDT. Ném BadRequestException nếu mã CTDT đã tồn tại.</summary>
    public async Task<ChuongTrinhDTDto> CreateAsync(UpsertChuongTrinhDTDto dto)
    {
        if (await uow.ChuongTrinhDTs.GetByMaCtdtAsync(dto.MaCtdt) is not null)
            throw new BadRequestException($"Mã CTDT '{dto.MaCtdt}' đã tồn tại.");

        var ctdt = new ChuongTrinhDT
        {
            MaCtdt   = dto.MaCtdt,
            NganhId  = dto.NganhId,
            KhoaHoc  = dto.KhoaHoc
        };

        await uow.ChuongTrinhDTs.AddAsync(ctdt);
        await uow.CommitAsync();

        return mapper.Map<ChuongTrinhDTDto>(await uow.ChuongTrinhDTs.GetDetailAsync(ctdt.Id));
    }

    /// <summary>Cập nhật CTDT. Kiểm tra mã CTDT mới không trùng với CTDT khác.</summary>
    public async Task<ChuongTrinhDTDto> UpdateAsync(int id, UpsertChuongTrinhDTDto dto)
    {
        var ctdt = await uow.ChuongTrinhDTs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy chương trình đào tạo id = {id}.");

        var existing = await uow.ChuongTrinhDTs.GetByMaCtdtAsync(dto.MaCtdt);
        if (existing is not null && existing.Id != id)
            throw new BadRequestException($"Mã CTDT '{dto.MaCtdt}' đã được sử dụng.");

        ctdt.MaCtdt  = dto.MaCtdt;
        ctdt.NganhId = dto.NganhId;
        ctdt.KhoaHoc = dto.KhoaHoc;

        uow.ChuongTrinhDTs.Update(ctdt);
        await uow.CommitAsync();

        return mapper.Map<ChuongTrinhDTDto>(await uow.ChuongTrinhDTs.GetDetailAsync(id));
    }

    /// <summary>Xoá CTDT. Ném BadRequestException nếu còn lớp sinh hoạt đang dùng CTDT này.</summary>
    public async Task DeleteAsync(int id)
    {
        var ctdt = await uow.ChuongTrinhDTs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy chương trình đào tạo id = {id}.");

        var coLop = await uow.LopSinhHoats.GetPagedFilterAsync(1, 1, null, null);
        // Kiểm tra có lớp sinh hoạt nào gắn CTDT này không
        var coLopCtdt = await uow.ChiTietCTDTs.GetPagedFilterAsync(1, 1, id);
        if (coLopCtdt.Total > 0)
            throw new BadRequestException("Không thể xoá CTDT đang có chi tiết môn học liên kết.");

        uow.ChuongTrinhDTs.Delete(ctdt);
        await uow.CommitAsync();
    }
}
