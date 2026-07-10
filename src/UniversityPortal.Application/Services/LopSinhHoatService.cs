// Dịch vụ quản lý lớp sinh hoạt — xử lý circular ref thư ký khi tạo mới
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.LopSinhHoat;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ quản lý lớp sinh hoạt.
/// Circular reference: SinhVien.LopId → LopSinhHoat.ThuKyId → SinhVien
/// Giải pháp: tạo lớp với ThuKyId = null, gán thư ký qua UpdateAsync sau khi sinh viên đã có lớp.
/// </summary>
public class LopSinhHoatService(IUnitOfWork uow, IMapper mapper) : ILopSinhHoatService
{
    /// <summary>Lấy danh sách lớp sinh hoạt có phân trang và lọc.</summary>
    public async Task<PagedResultDto<LopSinhHoatDto>> GetPagedAsync(
        int page, int pageSize, string? keyword, int? gvcnId)
    {
        var paged = await uow.LopSinhHoats.GetPagedFilterAsync(page, pageSize, keyword, gvcnId);
        return new PagedResultDto<LopSinhHoatDto>
        {
            Data     = mapper.Map<IEnumerable<LopSinhHoatDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <summary>
    /// Lấy tất cả lớp sinh hoạt (không phân trang, dùng cho dropdown).
    /// Kèm Ngành/Khoa/Khoá học (qua ChuongTrinhDT) để client lọc dropdown khi thêm sinh viên.
    /// </summary>
    public async Task<IEnumerable<LopSinhHoatDto>> GetAllAsync()
        => mapper.Map<IEnumerable<LopSinhHoatDto>>(await uow.LopSinhHoats.GetAllDetailAsync());

    /// <summary>Lấy chi tiết lớp sinh hoạt theo id. Ném NotFoundException nếu không tồn tại.</summary>
    public async Task<LopSinhHoatDto> GetByIdAsync(int id)
    {
        var lop = await uow.LopSinhHoats.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy lớp sinh hoạt id = {id}.");
        return mapper.Map<LopSinhHoatDto>(lop);
    }

    /// <summary>
    /// Tạo mới lớp sinh hoạt.
    /// ThuKyId luôn = null khi tạo vì sinh viên cần có lớp trước khi được chỉ định làm thư ký.
    /// </summary>
    public async Task<LopSinhHoatDto> CreateAsync(CreateLopSinhHoatDto dto)
    {
        if (await uow.LopSinhHoats.GetByMaLopAsync(dto.MaLop) is not null)
            throw new BadRequestException($"Mã lớp '{dto.MaLop}' đã tồn tại.");

        var lop = new LopSinhHoat
        {
            MaLop           = dto.MaLop,
            GvcnId          = dto.GvcnId,
            ThuKyId         = null, // Gán sau khi sinh viên đã được thêm vào lớp
            ChuongTrinhDtId = dto.ChuongTrinhDtId
        };

        await uow.LopSinhHoats.AddAsync(lop);
        await uow.CommitAsync();

        return mapper.Map<LopSinhHoatDto>(await uow.LopSinhHoats.GetDetailAsync(lop.Id));
    }

    /// <summary>
    /// Cập nhật lớp sinh hoạt.
    /// Khi gán ThuKyId, kiểm tra sinh viên đó có thuộc lớp này không để tránh dữ liệu sai.
    /// </summary>
    public async Task<LopSinhHoatDto> UpdateAsync(int id, UpdateLopSinhHoatDto dto)
    {
        var lop = await uow.LopSinhHoats.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy lớp sinh hoạt id = {id}.");

        // Kiểm tra mã lớp mới không trùng với lớp khác
        var existing = await uow.LopSinhHoats.GetByMaLopAsync(dto.MaLop);
        if (existing is not null && existing.Id != id)
            throw new BadRequestException($"Mã lớp '{dto.MaLop}' đã được sử dụng.");

        // Kiểm tra thư ký được chỉ định phải thuộc lớp này
        if (dto.ThuKyId.HasValue)
        {
            var thuKy = await uow.SinhViens.GetByIdAsync(dto.ThuKyId.Value);
            if (thuKy is null || thuKy.LopId != id)
                throw new BadRequestException("Thư ký phải là sinh viên thuộc lớp này.");
        }

        lop.MaLop           = dto.MaLop;
        lop.GvcnId          = dto.GvcnId;
        lop.ThuKyId         = dto.ThuKyId;
        lop.ChuongTrinhDtId = dto.ChuongTrinhDtId;

        uow.LopSinhHoats.Update(lop);
        await uow.CommitAsync();

        return mapper.Map<LopSinhHoatDto>(await uow.LopSinhHoats.GetDetailAsync(id));
    }

    /// <summary>Xoá lớp sinh hoạt. Ném BadRequestException nếu còn sinh viên trong lớp.</summary>
    public async Task DeleteAsync(int id)
    {
        var lop = await uow.LopSinhHoats.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy lớp sinh hoạt id = {id}.");

        if (lop.SinhViens.Any())
            throw new BadRequestException("Không thể xoá lớp còn sinh viên. Chuyển sinh viên sang lớp khác trước.");

        uow.LopSinhHoats.Delete(lop);
        await uow.CommitAsync();
    }

    /// <summary>Lấy chi tiết lớp sinh hoạt của sinh viên đang đăng nhập, kèm roster bạn cùng lớp.</summary>
    public async Task<LopSinhHoatDto> GetMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        if (sv.LopId is null)
            throw new NotFoundException("Bạn chưa được phân vào lớp sinh hoạt nào.");

        var lop = await uow.LopSinhHoats.GetDetailAsync(sv.LopId.Value)
            ?? throw new NotFoundException($"Không tìm thấy lớp sinh hoạt id = {sv.LopId}.");

        return mapper.Map<LopSinhHoatDto>(lop);
    }

    /// <summary>Lấy (các) lớp mà giáo viên đang đăng nhập là GVCN.</summary>
    public async Task<IEnumerable<LopSinhHoatDto>> GetMeGvcnAsync(int taiKhoanId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var lops = await uow.LopSinhHoats.GetByGvcnIdAsync(gv.Id);
        return mapper.Map<IEnumerable<LopSinhHoatDto>>(lops);
    }
}
