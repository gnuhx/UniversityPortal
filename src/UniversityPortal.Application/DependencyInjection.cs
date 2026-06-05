using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Application.Mappings;
using UniversityPortal.Application.Services;

namespace UniversityPortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
