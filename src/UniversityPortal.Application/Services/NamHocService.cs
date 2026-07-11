// Dịch vụ quản lý năm học — CRUD với kiểm tra trùng tên và ràng buộc xoá
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.NamHoc;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý năm học.
/// </summary>
public class NamHocService(IUnitOfWork uow, IMapper mapper) : INamHocService
{
    /// <summary>Lấy danh sách năm học có phân trang và lọc keyword.</summary>
    public async Task<PagedResultDto<NamHocDto>> GetPagedAsync(int page, int pageSize, string? keyword)
    {
        var paged = await uow.NamHocs.GetPagedFilterAsync(page, pageSize, keyword);
        return new PagedResultDto<NamHocDto>
        {
            Data     = mapper.Map<IEnumerable<NamHocDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy tất cả năm học (không phân trang, dùng cho dropdown).</summary>
    public async Task<IEnumerable<NamHocDto>> GetAllAsync()
        => mapper.Map<IEnumerable<NamHocDto>>(await uow.NamHocs.GetAllAsync());

    /// <summary>Lấy chi tiết năm học theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<NamHocDto> GetByIdAsync(int id)
    {
        var namHoc = await uow.NamHocs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy năm học id = {id}.");
        return mapper.Map<NamHocDto>(namHoc);
    }

    /// <summary>Tạo mới năm học. Ném BadRequestException nếu tên năm học đã tồn tại.</summary>
    public async Task<NamHocDto> CreateAsync(UpsertNamHocDto dto)
    {
        if (await uow.NamHocs.GetByTenNamHocAsync(dto.TenNamHoc) is not null)
            throw new BadRequestException($"Năm học '{dto.TenNamHoc}' đã tồn tại.");

        var namHoc = new NamHoc { TenNamHoc = dto.TenNamHoc };

        await uow.NamHocs.AddAsync(namHoc);
        await uow.CommitAsync();

        return mapper.Map<NamHocDto>(namHoc);
    }

    /// <summary>Cập nhật năm học. Kiểm tra tên mới không trùng với năm học khác.</summary>
    public async Task<NamHocDto> UpdateAsync(int id, UpsertNamHocDto dto)
    {
        var namHoc = await uow.NamHocs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy năm học id = {id}.");

        var existing = await uow.NamHocs.GetByTenNamHocAsync(dto.TenNamHoc);
        if (existing is not null && existing.Id != id)
            throw new BadRequestException($"Năm học '{dto.TenNamHoc}' đã được sử dụng.");

        namHoc.TenNamHoc = dto.TenNamHoc;

        uow.NamHocs.Update(namHoc);
        await uow.CommitAsync();

        return mapper.Map<NamHocDto>(namHoc);
    }

    /// <summary>Xoá năm học. Ném BadRequestException nếu còn học kỳ hoặc tuần học liên kết.</summary>
    public async Task DeleteAsync(int id)
    {
        var namHoc = await uow.NamHocs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy năm học id = {id}.");

        if (await uow.HocKys.ExistsByNamHocAsync(id))
            throw new BadRequestException("Không thể xoá năm học đang có học kỳ liên kết.");

        if (await uow.TuanHocs.ExistsByNamHocAsync(id))
            throw new BadRequestException("Không thể xoá năm học đang có tuần học liên kết.");

        uow.NamHocs.Delete(namHoc);
        await uow.CommitAsync();
    }
}
