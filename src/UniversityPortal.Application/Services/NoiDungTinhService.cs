using UniversityPortal.Application.DTOs.NoiDungTinh;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class NoiDungTinhService(IUnitOfWork uow) : INoiDungTinhService
{
    public async Task<IEnumerable<NoiDungTinhDto>> GetByKhuVucAsync(string khuVuc)
        => (await uow.NoiDungTinhs.GetByKhuVucAsync(khuVuc)).Select(MapToDto);

    public async Task<NoiDungTinhDto> GetByIdAsync(int id)
    {
        var nd = await uow.NoiDungTinhs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy mục nội dung id = {id}.");
        return MapToDto(nd);
    }

    public async Task<NoiDungTinhDto> CreateAsync(UpsertNoiDungTinhDto dto)
    {
        var cungKhuVuc = await uow.NoiDungTinhs.GetByKhuVucAsync(dto.KhuVuc);
        if (cungKhuVuc.Any(x => x.MaMuc == dto.MaMuc))
            throw new BadRequestException($"Mục '{dto.MaMuc}' đã tồn tại trong khu vực '{dto.KhuVuc}'.");

        var nd = new NoiDungTinh
        {
            KhuVuc  = dto.KhuVuc,
            MaMuc   = dto.MaMuc,
            TieuDe  = dto.TieuDe,
            NoiDung = dto.NoiDung,
            ThuTu   = dto.ThuTu,
        };

        await uow.NoiDungTinhs.AddAsync(nd);
        await uow.CommitAsync();

        return MapToDto(nd);
    }

    public async Task<NoiDungTinhDto> UpdateAsync(int id, UpsertNoiDungTinhDto dto)
    {
        var nd = await uow.NoiDungTinhs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy mục nội dung id = {id}.");

        var cungKhuVuc = await uow.NoiDungTinhs.GetByKhuVucAsync(dto.KhuVuc);
        if (cungKhuVuc.Any(x => x.MaMuc == dto.MaMuc && x.Id != id))
            throw new BadRequestException($"Mục '{dto.MaMuc}' đã tồn tại trong khu vực '{dto.KhuVuc}'.");

        nd.KhuVuc  = dto.KhuVuc;
        nd.MaMuc   = dto.MaMuc;
        nd.TieuDe  = dto.TieuDe;
        nd.NoiDung = dto.NoiDung;
        nd.ThuTu   = dto.ThuTu;

        uow.NoiDungTinhs.Update(nd);
        await uow.CommitAsync();

        return MapToDto(nd);
    }

    public async Task DeleteAsync(int id)
    {
        var nd = await uow.NoiDungTinhs.GetByIdAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy mục nội dung id = {id}.");

        uow.NoiDungTinhs.Delete(nd);
        await uow.CommitAsync();
    }

    private static NoiDungTinhDto MapToDto(NoiDungTinh nd) => new()
    {
        Id        = nd.Id,
        KhuVuc    = nd.KhuVuc,
        MaMuc     = nd.MaMuc,
        TieuDe    = nd.TieuDe,
        NoiDung   = nd.NoiDung,
        ThuTu     = nd.ThuTu,
        CreatedAt = nd.CreatedAt,
    };
}
