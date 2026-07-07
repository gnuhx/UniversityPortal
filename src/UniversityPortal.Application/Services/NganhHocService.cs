// Dịch vụ quản lý ngành học — CRUD với kiểm tra trùng mã và ràng buộc xoá
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NganhHoc;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý ngành học.
/// </summary>
public class NganhHocService(IUnitOfWork uow, IMapper mapper) : INganhHocService
{
    /// <summary>Lấy danh sách ngành học có phân trang, lọc keyword, ngành (id) và khoá học.</summary>
    public async Task<PagedResultDto<NganhHocDto>> GetPagedAsync(int page, int pageSize, string? keyword, int? nganhId = null, string? khoaHoc = null)
    {
        var paged = await uow.NganhHocs.GetPagedFilterAsync(page, pageSize, keyword, nganhId, khoaHoc);
        return new PagedResultDto<NganhHocDto>
        {
            Data     = mapper.Map<IEnumerable<NganhHocDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy tất cả ngành học (không phân trang, dùng cho dropdown).</summary>
    public async Task<IEnumerable<NganhHocDto>> GetAllAsync()
        => mapper.Map<IEnumerable<NganhHocDto>>(await uow.NganhHocs.GetAllAsync());

    /// <summary>Lấy chi tiết ngành học theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<NganhHocDto> GetByIdAsync(int id)
    {
        var nganh = await uow.NganhHocs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy ngành học id = {id}.");
        return mapper.Map<NganhHocDto>(nganh);
    }

    /// <summary>Tạo mới ngành học. Ném BadRequestException nếu mã ngành đã tồn tại.</summary>
    public async Task<NganhHocDto> CreateAsync(UpsertNganhHocDto dto)
    {
        if (await uow.NganhHocs.GetByMaNganhAsync(dto.MaNganh) is not null)
            throw new BadRequestException($"Mã ngành '{dto.MaNganh}' đã tồn tại.");

        var nganh = new NganhHoc
        {
            MaNganh    = dto.MaNganh,
            TenNganh   = dto.TenNganh,
            NganhChaId = dto.NganhChaId,
            PhongBanId = dto.PhongBanId
        };

        await uow.NganhHocs.AddAsync(nganh);
        await uow.CommitAsync();

        return mapper.Map<NganhHocDto>(await uow.NganhHocs.GetDetailAsync(nganh.Id));
    }

    /// <summary>Cập nhật ngành học. Kiểm tra mã ngành mới không trùng với ngành khác.</summary>
    public async Task<NganhHocDto> UpdateAsync(int id, UpsertNganhHocDto dto)
    {
        var nganh = await uow.NganhHocs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy ngành học id = {id}.");

        var existing = await uow.NganhHocs.GetByMaNganhAsync(dto.MaNganh);
        if (existing is not null && existing.Id != id)
            throw new BadRequestException($"Mã ngành '{dto.MaNganh}' đã được sử dụng.");

        nganh.MaNganh    = dto.MaNganh;
        nganh.TenNganh   = dto.TenNganh;
        nganh.NganhChaId = dto.NganhChaId;
        nganh.PhongBanId = dto.PhongBanId;

        uow.NganhHocs.Update(nganh);
        await uow.CommitAsync();

        return mapper.Map<NganhHocDto>(await uow.NganhHocs.GetDetailAsync(id));
    }

    /// <summary>Xoá ngành học. Ném BadRequestException nếu còn CTDT đang dùng ngành này.</summary>
    public async Task DeleteAsync(int id)
    {
        var nganh = await uow.NganhHocs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy ngành học id = {id}.");

        // Kiểm tra ràng buộc trước khi xoá để tránh lỗi FK từ DB
        var coCtdt = await uow.ChuongTrinhDTs.GetPagedFilterAsync(1, 1, null, id, null);
        if (coCtdt.Total > 0)
            throw new BadRequestException("Không thể xoá ngành đang có chương trình đào tạo liên kết.");

        uow.NganhHocs.Delete(nganh);
        await uow.CommitAsync();
    }
}
