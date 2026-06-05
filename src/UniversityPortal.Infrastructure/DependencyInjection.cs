// Đăng ký toàn bộ services của Infrastructure layer vào DI container
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Infrastructure.Persistence;
using UniversityPortal.Infrastructure.Services;

namespace UniversityPortal.Infrastructure;

/// <summary>
/// Extension method đăng ký Infrastructure layer.
/// Gọi trong Program.cs: builder.Services.AddInfrastructure(config)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

        // MySQL 8.0 — Aiven dùng MySQL 8.x, retry on failure cho Docker startup race condition
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connStr, serverVersion,
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // FileService xử lý lưu/xoá ảnh đại diện trên filesystem
        services.AddScoped<IFileService, FileService>();

        return services;
    }
}
