namespace Maros.Domain.Enums;

// Campo simple por ahora — la tabla de permisos granular (RolePermission)
// se construye en Fase 4/5, junto con la autenticación real.
public enum UserRole
{
    Administrador,
    Editor,
    Viewer
}