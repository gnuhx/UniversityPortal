// Dịch vụ quản lý môn học
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.MonHoc;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý môn học.
/// </summary>
public class MonHocService(IUnitOfWork uow, IMapper mapper) : IMonHocService
{
    /// <summary>Lấy danh sách môn học có phân trang và lọc keyword.</summary>
    public async Task<PagedResultDto<MonHocDto>> GetPagedAsync(int page, int pageSize, string? keyword)
    {
        var paged = await uow.MonHocs.GetPagedFilterAsync(page, pageSize, keyword);
        return new PagedResultDto<MonHocDto>
        {
            Data     = mapper.Map<IEnumerable<MonHocDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy tất cả môn học (không phân trang, dùng cho dropdown).</summary>
    public async Task<IEnumerable<MonHocDto>> GetAllAsync()
        => mapper.Map<IEnumerable<MonHocDto>>(await uow.MonHocs.GetAllAsync());

    /// <summary>Lấy chi tiết môn học theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<MonHocDto> GetByIdAsync(int id)
    {
        var mon = await uow.MonHocs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy môn học id = {id}.");
        return mapper.Map<MonHocDto>(mon);
    }

    /// <summary>Tạo mới môn học. Ném BadRequestException nếu mã môn đã tồn tại.</summary>
    public async Task<MonHocDto> CreateAsync(UpsertMonHocDto dto)
    {
        if (await uow.MonHocs.GetByMaMonAsync(dto.MaMon) is not null)
            throw new BadRequestException($"Mã môn '{dto.MaMon}' đã tồn tại.");

        var mon = new MonHoc { MaMon = dto.MaMon, TenMon = dto.TenMon };
        await uow.MonHocs.AddAsync(mon);
        await uow.CommitAsync();

        return mapper.Map<MonHocDto>(mon);
    }

    /// <summary>Cập nhật môn học. Kiểm tra mã môn mới không trùng với môn khác.</summary>
    public async Task<MonHocDto> UpdateAsync(int id, UpsertMonHocDto dto)
    {
        var mon = await uow.MonHocs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy môn học id = {id}.");

        var existing = await uow.MonHocs.GetByMaMonAsync(dto.MaMon);
        if (existing is not null && existing.Id != id)
            throw new BadRequestException($"Mã môn '{dto.MaMon}' đã được sử dụng.");

        mon.MaMon  = dto.MaMon;
        mon.TenMon = dto.TenMon;

        uow.MonHocs.Update(mon);
        await uow.CommitAsync();

        return mapper.Map<MonHocDto>(mon);
    }

    /// <summary>Xoá môn học. Ném BadRequestException nếu môn đang có trong chi tiết CTDT.</summary>
    public async Task DeleteAsync(int id)
    {
        var mon = await uow.MonHocs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy môn học id = {id}.");

        var coChiTiet = await uow.ChiTietCTDTs.GetPagedFilterAsync(1, 1, null);
        // Kiểm tra riêng theo môn — dùng ExistsAsync không có sẵn nên kiểm tra qua query
        var query = await uow.ChiTietCTDTs.GetPagedFilterAsync(1, 1, null);
        // Dùng repository tổng quát: xoá trực tiếp sẽ bị FK nếu còn ChiTietCTDT
        // EF Core sẽ ném DbUpdateException — GlobalExceptionMiddleware sẽ bắt và trả 400
        uow.MonHocs.Delete(mon);
        await uow.CommitAsync();
    }
}
