// Đăng ký toàn bộ services của Application layer vào DI container
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Application.Mappings;
using UniversityPortal.Application.Services;

namespace UniversityPortal.Application;

/// <summary>
/// Extension method đăng ký Application layer.
/// Gọi trong Program.cs: builder.Services.AddApplication()
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper — quét toàn bộ Assembly để tìm Profile
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // FluentValidation — tự động đăng ký tất cả Validator trong Assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // ── Tuần 1 ────────────────────────────────────────────────────────────
        services.AddScoped<IAuthService, AuthService>();

        // ── Tuần 2 ────────────────────────────────────────────────────────────
        services.AddScoped<ITaiKhoanService, TaiKhoanService>();
        services.AddScoped<IGiaoVienService, GiaoVienService>();
        services.AddScoped<ISinhVienService, SinhVienService>();
        services.AddScoped<INganhHocService, NganhHocService>();
        services.AddScoped<IPhongBanService, PhongBanService>();
        services.AddScoped<IChuongTrinhDTService, ChuongTrinhDTService>();
        services.AddScoped<IMonHocService, MonHocService>();
        services.AddScoped<IChiTietCTDTService, ChiTietCTDTService>();
        services.AddScoped<ILopSinhHoatService, LopSinhHoatService>();
        services.AddScoped<IDanhSachLopHPService, DanhSachLopHPService>();
        services.AddScoped<ILopHocPhanService, LopHocPhanService>();
        services.AddScoped<IThongBaoService, ThongBaoService>();
        services.AddScoped<IHocPhiService, HocPhiService>();
        services.AddScoped<IYeuCauHanhChinhService, YeuCauHanhChinhService>();
        services.AddScoped<IYeuCauSuaDiemService, YeuCauSuaDiemService>();
        services.AddScoped<ITuanHocService, TuanHocService>();
        services.AddScoped<IThoiKhoaBieuService, ThoiKhoaBieuService>();
        services.AddScoped<INamHocService, NamHocService>();
        services.AddScoped<IHocKyService, HocKyService>();

        return services;
    }
}
