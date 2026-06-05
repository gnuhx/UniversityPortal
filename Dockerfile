FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY UniversityPortal.slnx ./
COPY src/UniversityPortal.Domain/UniversityPortal.Domain.csproj src/UniversityPortal.Domain/
COPY src/UniversityPortal.Application/UniversityPortal.Application.csproj src/UniversityPortal.Application/
COPY src/UniversityPortal.Infrastructure/UniversityPortal.Infrastructure.csproj src/UniversityPortal.Infrastructure/
COPY src/UniversityPortal.API/UniversityPortal.API.csproj src/UniversityPortal.API/

RUN dotnet restore src/UniversityPortal.API/UniversityPortal.API.csproj

COPY src/ src/

RUN dotnet publish src/UniversityPortal.API/UniversityPortal.API.csproj \
    -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "UniversityPortal.API.dll"]
