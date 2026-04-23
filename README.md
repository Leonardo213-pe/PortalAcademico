# Portal Académico — Universidad

## Descripción
Portal web para gestión de cursos y matrículas universitarias.
Stack: ASP.NET Core MVC .NET 8 + Identity + EF Core SQLite + Redis

## Pasos para correr localmente

### Requisitos
- .NET 8 SDK
- Git

### Instalación
```bash
git clone https://github.com/Leonardo213-pe/PortalAcademico.git
cd PortalAcademico/PortalAcademicoWeb
dotnet restore
```

### Migraciones
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Correr el proyecto
```bash
dotnet run
```

Abrir en el navegador: `http://localhost:5000`

## Variables de entorno

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | `Production` |
| `ASPNETCORE_URLS` | URL de escucha | `http://0.0.0.0:10000` |
| `ConnectionStrings__DefaultConnection` | Cadena SQLite | `Data Source=/data/portal_academico.db` |
| `Redis__ConnectionString` | URL Redis | `redis://user:pass@host:port` |

## Credenciales iniciales
- **Coordinador:** `coordinador@universidad.edu` / `Admin123!`

## Ramas del proyecto
| Rama | Descripción |
|------|-------------|
| `feature/bootstrap-dominio` | P1: Modelos y configuración base |
| `feature/catalogo-cursos` | P2: Catálogo con filtros |
| `feature/matriculas` | P3: Inscripciones y validaciones |
| `feature/sesion-redis` | P4: Sesiones y caché Redis |
| `feature/panel-coordinador` | P5: Panel de coordinador |
| `deploy/render` | P6: Deploy en Render |

## URL en Render
https://portalacademico.onrender.com