FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para restaurar caché de capas
COPY backend/Maros.Domain/Maros.Domain.csproj backend/Maros.Domain/
COPY backend/Maros.Application/Maros.Application.csproj backend/Maros.Application/
COPY backend/Maros.Infrastructure/Maros.Infrastructure.csproj backend/Maros.Infrastructure/
COPY backend/Maros.Api/Maros.Api.csproj backend/Maros.Api/

RUN dotnet restore backend/Maros.Api/Maros.Api.csproj

# Copiar código fuente y compilar
COPY backend/ ./backend/
RUN dotnet publish backend/Maros.Api/Maros.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Render (y docker compose) inyectan $PORT. Si no está definido, usa 8080.
ENTRYPOINT ["sh", "-c", "dotnet Maros.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
