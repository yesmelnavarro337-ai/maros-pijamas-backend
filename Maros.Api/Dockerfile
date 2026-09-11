# ─────────────────────────────────────────────────────────────
# Maro's Pijamas API — Dockerfile
# Build context: backend/  (docker build -f Maros.Api/Dockerfile .)
# ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar los .csproj para aprovechar la caché de capas de restore
COPY Maros.Api/Maros.Api.csproj Maros.Api/
COPY Maros.Application/Maros.Application.csproj Maros.Application/
COPY Maros.Infrastructure/Maros.Infrastructure.csproj Maros.Infrastructure/
COPY Maros.Domain/Maros.Domain.csproj Maros.Domain/

RUN dotnet restore Maros.Api/Maros.Api.csproj

# Copiar código fuente y publicar
COPY . .
RUN dotnet publish Maros.Api/Maros.Api.csproj -c Release -o /app/publish --no-restore

# ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Render inyecta $PORT; si no está definido se usa el puerto por defecto 8080.
ENTRYPOINT ["sh", "-c", "dotnet Maros.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]