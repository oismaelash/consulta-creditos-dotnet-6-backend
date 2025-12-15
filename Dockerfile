# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/ConsultaCreditos.Api/ConsultaCreditos.Api.csproj", "src/ConsultaCreditos.Api/"]
COPY ["src/ConsultaCreditos.Application/ConsultaCreditos.Application.csproj", "src/ConsultaCreditos.Application/"]
COPY ["src/ConsultaCreditos.Domain/ConsultaCreditos.Domain.csproj", "src/ConsultaCreditos.Domain/"]
COPY ["src/ConsultaCreditos.Infrastructure/ConsultaCreditos.Infrastructure.csproj", "src/ConsultaCreditos.Infrastructure/"]

RUN dotnet restore "src/ConsultaCreditos.Api/ConsultaCreditos.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/ConsultaCreditos.Api"
RUN dotnet build "ConsultaCreditos.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ConsultaCreditos.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ConsultaCreditos.Api.dll"]

