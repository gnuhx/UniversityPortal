using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

        // MySQL 8.0 — Aiven uses MySQL 8.x
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connStr, serverVersion));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
