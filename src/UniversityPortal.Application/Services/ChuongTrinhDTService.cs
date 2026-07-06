// Dịch vụ quản lý chương trình đào tạo
using System.Text.RegularExpressions;
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

    /// <summary>
    /// Nhân bản CTDT có khoá học mới nhất của 1 ngành sang khoá học mới. Môn học được ánh xạ
    /// theo vị trí tương đối (năm thứ mấy trong khoá + thứ tự học kỳ trong năm đó) chứ không
    /// sao chép nguyên `hocKyId` — vì học kỳ là mốc thời gian tuyệt đối, sao chép nguyên sẽ gán
    /// nhầm môn học của khoá mới vào các học kỳ đã qua của khoá cũ. Môn nào không tìm được học kỳ
    /// tương ứng ở khoá mới (vd năm học đó chưa được tạo) sẽ bị bỏ qua và liệt kê trong kết quả.
    /// </summary>
    public async Task<CloneChuongTrinhDTResultDto> CloneAsync(CloneChuongTrinhDTDto dto)
    {
        var allCtdtCuaNganh = await uow.ChuongTrinhDTs.GetPagedFilterAsync(1, int.MaxValue, null, dto.NganhId, null);
        var nguon = allCtdtCuaNganh.Data
            .OrderByDescending(x => x.KhoaHoc, StringComparer.Ordinal)
            .FirstOrDefault()
            ?? throw new BadRequestException("Ngành này chưa có chương trình đào tạo nào để nhân bản.");

        var namBatDauNguon = ParseNamBatDau(nguon.KhoaHoc);
        var namBatDauMoi = ParseNamBatDau(dto.KhoaHocMoi);
        if (namBatDauNguon is null || namBatDauMoi is null)
            throw new BadRequestException("Khoá học phải theo định dạng \"YYYY-YYYY\" để nhân bản đúng học kỳ tương ứng.");

        if (await uow.ChuongTrinhDTs.GetByMaCtdtAsync(dto.MaCtdtMoi) is not null)
            throw new BadRequestException($"Mã CTDT '{dto.MaCtdtMoi}' đã tồn tại.");

        var ctdtMoi = new ChuongTrinhDT { MaCtdt = dto.MaCtdtMoi, NganhId = dto.NganhId, KhoaHoc = dto.KhoaHocMoi };
        await uow.ChuongTrinhDTs.AddAsync(ctdtMoi);
        await uow.CommitAsync();

        var tatCaHocKy = (await uow.HocKys.GetAllAsync()).ToList();
        // ThenBy(Id) để có thứ tự ổn định khi 2 học kỳ trùng NgayBatDau (dữ liệu trùng lặp/không sạch) —
        // nếu không, vị trí thứ tự trong năm có thể đổi giữa các lần chạy, ánh xạ sai học kỳ đích.
        List<HocKy> HocKyCuaNam(int namBatDau) => tatCaHocKy
            .Where(hk => ParseNamBatDau(hk.NamHoc.TenNamHoc) == namBatDau)
            .OrderBy(hk => hk.NgayBatDau).ThenBy(hk => hk.Id)
            .ToList();

        var monHocNguon = await uow.ChiTietCTDTs.GetByCtdtIdAsync(nguon.Id);
        var soMonDaSaoChep = 0;
        var monBoQua = new List<string>();

        foreach (var mon in monHocNguon)
        {
            var namBatDauMonNguon = ParseNamBatDau(mon.HocKy.NamHoc.TenNamHoc);
            if (namBatDauMonNguon is null)
            {
                monBoQua.Add($"{mon.MonHoc.MaMon} (không xác định được năm học nguồn)");
                continue;
            }

            var hocKyCuaNamNguon = HocKyCuaNam(namBatDauMonNguon.Value);
            var thuTu = hocKyCuaNamNguon.FindIndex(hk => hk.Id == mon.HocKyId);

            var namBatDauDich = namBatDauMoi.Value + (namBatDauMonNguon.Value - namBatDauNguon.Value);
            var hocKyCuaNamDich = HocKyCuaNam(namBatDauDich);

            if (thuTu < 0 || thuTu >= hocKyCuaNamDich.Count)
            {
                monBoQua.Add($"{mon.MonHoc.MaMon} (chưa có học kỳ tương ứng ở năm học {namBatDauDich}-{namBatDauDich + 1})");
                continue;
            }

            await uow.ChiTietCTDTs.AddAsync(new ChiTietCTDT
            {
                CtdtId     = ctdtMoi.Id,
                MonHocId   = mon.MonHocId,
                HocKyId    = hocKyCuaNamDich[thuTu].Id,
                SoTinChi   = mon.SoTinChi,
                TinhDiemTb = mon.TinhDiemTb,
            });
            soMonDaSaoChep++;
        }

        if (soMonDaSaoChep > 0)
            await uow.CommitAsync();

        return new CloneChuongTrinhDTResultDto
        {
            CtdtMoiId      = ctdtMoi.Id,
            MaCtdtMoi      = ctdtMoi.MaCtdt,
            SoMonDaSaoChep = soMonDaSaoChep,
            MonBoQua       = monBoQua,
        };
    }

    /// <summary>Suy năm bắt đầu từ chuỗi "YYYY-YYYY" (khoá học hoặc tên năm học). Trả null nếu không đúng định dạng.</summary>
    private static int? ParseNamBatDau(string value)
    {
        var match = Regex.Match(value, @"^(\d{4})-(\d{4})$");
        return match.Success ? int.Parse(match.Groups[1].Value) : null;
    }
}
