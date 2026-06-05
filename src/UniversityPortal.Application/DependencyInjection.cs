using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UniversityPortal.Application.Mappings;

namespace UniversityPortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
