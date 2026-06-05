// Dịch vụ quản lý chi tiết chương trình đào tạo — thêm/sửa/xoá môn học trong CTDT
using AutoMapper;
using UniversityPortal.Application.DTOs.ChiTietCTDT;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý chi tiết chương trình đào tạo.
/// </summary>
public class ChiTietCTDTService(IUnitOfWork uow, IMapper mapper) : IChiTietCTDTService
{
    /// <summary>Lấy danh sách chi tiết CTDT có phân trang, có thể lọc theo CTDT.</summary>
    public async Task<PagedResultDto<ChiTietCTDTDto>> GetPagedAsync(int page, int pageSize, int? ctdtId)
    {
        var paged = await uow.ChiTietCTDTs.GetPagedFilterAsync(page, pageSize, ctdtId);
        return new PagedResultDto<ChiTietCTDTDto>
        {
            Data     = mapper.Map<IEnumerable<ChiTietCTDTDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Lấy chi tiết một dòng theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<ChiTietCTDTDto> GetByIdAsync(int id)
    {
        var ct = await uow.ChiTietCTDTs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy chi tiết CTDT id = {id}.");
        return mapper.Map<ChiTietCTDTDto>(ct);
    }

    /// <summary>
    /// Thêm môn học vào CTDT.
    /// Ném BadRequestException nếu môn học đó đã có trong CTDT (tránh trùng).
    /// </summary>
    public async Task<ChiTietCTDTDto> CreateAsync(CreateChiTietCTDTDto dto)
    {
        if (await uow.ChiTietCTDTs.ExistsAsync(dto.CtdtId, dto.MonHocId))
            throw new BadRequestException("Môn học này đã có trong chương trình đào tạo.");

        var ct = new ChiTietCTDT
        {
            CtdtId    = dto.CtdtId,
            MonHocId  = dto.MonHocId,
            HocKyId   = dto.HocKyId,
            SoTinChi  = dto.SoTinChi,
            TinhDiemTb = dto.TinhDiemTb
        };

        await uow.ChiTietCTDTs.AddAsync(ct);
        await uow.CommitAsync();

        return mapper.Map<ChiTietCTDTDto>(await uow.ChiTietCTDTs.GetDetailAsync(ct.Id));
    }

    /// <summary>Cập nhật số tín chỉ và cờ tính điểm TB của một dòng chi tiết CTDT.</summary>
    public async Task<ChiTietCTDTDto> UpdateAsync(int id, UpdateChiTietCTDTDto dto)
    {
        var ct = await uow.ChiTietCTDTs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy chi tiết CTDT id = {id}.");

        ct.SoTinChi   = dto.SoTinChi;
        ct.TinhDiemTb = dto.TinhDiemTb;

        uow.ChiTietCTDTs.Update(ct);
        await uow.CommitAsync();

        return mapper.Map<ChiTietCTDTDto>(await uow.ChiTietCTDTs.GetDetailAsync(id));
    }

    /// <summary>Xoá một dòng chi tiết khỏi CTDT.</summary>
    public async Task DeleteAsync(int id)
    {
        var ct = await uow.ChiTietCTDTs.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy chi tiết CTDT id = {id}.");

        uow.ChiTietCTDTs.Delete(ct);
        await uow.CommitAsync();
    }
}
