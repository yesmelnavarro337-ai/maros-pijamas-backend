# Arquitectura — Maro's Pijamas Backend

## Stack
- .NET 10 / ASP.NET Core Web API
- SQL Server (LocalDB en desarrollo)
- Entity Framework Core 10 (Code First + Migrations)
- JWT Bearer Authentication
- Cloudinary (almacenamiento de imágenes)

## Estructura de capas (Clean Architecture)

**Regla de dependencias:**
`Domain` nunca depende de nada. `Application` nunca depende de EF Core ni de ningún paquete de infraestructura — solo de `Domain` y de abstracciones de DI.

## Patrón por módulo

Cada recurso de negocio (Products, Seasons, Quotations, etc.) sigue el mismo patrón de 4 piezas:

1. **Entity** (`Domain/Entities`) — modelo de datos puro.
2. **I{Recurso}Repository** (`Domain/Interfaces`) — contrato de persistencia.
3. **I{Recurso}Service + {Recurso}Service** (`Application`) — lógica de negocio, validaciones, orquestación. Usa DTOs, nunca expone entidades.
4. **{Recurso}Repository** (`Infrastructure`) — implementación EF Core del contrato.
5. **{Recurso}Controller** (`Api`) — HTTP thin layer, delega todo al Service.

## Autenticación y autorización

- JWT con claims: `sub` (UserId), `email`, `name`, `role`.
- Policies: `RequireAdministrador`, `RequireEditorOrAdmin`.
- Rutas bajo `/api/public/*` son anónimas por diseño — nunca deben requerir autenticación.
- Rutas administrativas usan `[Authorize]` a nivel de controller, con `[Authorize(Policy = "...")]` puntual en acciones de escritura cuando el rol Editor no debe tenerla (ej. Configuración, Usuarios).

## Manejo de errores

Todo error pasa por `ExceptionHandlingMiddleware` (`Api/Middleware`), que devuelve siempre el formato:
```json
{ "status": number, "message": string, "errors": { "campo": ["mensaje"] } | null }
```
- `AppException` (en `Application/Common`) → errores de negocio esperados (400/403/404/409). Los servicios la lanzan explícitamente.
- Fallos de validación automática de DTOs (`[Required]`, etc.) → se reformatean al mismo contrato en `Program.cs` (`InvalidModelStateResponseFactory`).
- `DbUpdateException` → se traduce a 409 genérico, nunca se expone el mensaje SQL crudo.
- Cualquier otra excepción → 500 genérico, se registra el detalle en logs, nunca se expone al cliente.

## Reglas de negocio clave a recordar

- **Temporadas**: solo una puede estar `Activa` a la vez (`SeasonService.ActivateAsync` + `ISeasonRepository.DeactivateAllExceptAsync`).
- **Colección pública activa**: si hay una Temporada activa, se usa su colección; si no, se usa la Colección marcada `IsDefault` (`CollectionService.GetActivePublicAsync`).
- **Banners públicos**: visibles si `Active=true` Y dentro de su rango de fechas (si tiene) Y (si está vinculado a una Temporada, esa Temporada debe estar Activa).
- **SKU de variantes de producto**: únicos globalmente en todo el catálogo, no solo dentro del mismo producto.
- **Clientes**: se deduplican por número de teléfono al crear una cotización pública (`QuotationService.CreateAsync`).
- **CustomizationOption**: qué campos aplican (imagen/color/recargo) depende del `CatalogType`, reglas centralizadas en `CustomizationCatalogRules`.

## Paginación

Recursos con volumen creciente (Products, Quotations, Customers, Gallery, Blog) usan `PagedResult<T>` + `PagedQueryParams`. La ejecución real de `Skip/Take` vive en `Infrastructure/Services/PaginationService` (implementa `IPaginationService` de `Application`) — nunca uses EF Core directamente dentro de `Application`.

## Pendientes conocidos (fuera de alcance actual)

- Recuperación de contraseña real (solo hay placeholder en frontend).
- WhatsApp Business API (se generó solo el link `wa.me`, sin integración de envío automático).
- Auditoría de acciones administrativas (mencionada como "posible" en el plan original, no implementada).
- Tests de integración HTTP completos (autorización por rol vía `WebApplicationFactory`) — hoy solo hay tests unitarios de `Application`.

## Cómo correr el proyecto localmente

```powershell
cd backend
dotnet user-secrets list --project Maros.Api   # verificar que existan los secretos necesarios
dotnet ef database update --project Maros.Infrastructure --startup-project Maros.Api
dotnet run --project Maros.Api
```
Documentación interactiva: `http://localhost:5072/`