
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Devices.Api/Devices.Api.csproj", "src/Devices.Api/"]
COPY ["src/Decives.Infrastructure/Decives.Infrastructure.csproj", "src/Decives.Infrastructure/"]
COPY ["src/Devices.Application/Devices.Application.csproj", "src/Devices.Application/"]
COPY ["src/Devices.Domain/Devices.Domain.csproj", "src/Devices.Domain/"]

RUN dotnet restore "./src/Devices.Api/Devices.Api.csproj"

COPY . .
WORKDIR "/src/src/Devices.Api"
RUN dotnet build "./Devices.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Devices.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Devices.Api.dll"]