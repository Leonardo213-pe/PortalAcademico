# Portal Académico — Universidad

# Descripción
# Portal web para gestión de cursos y matrículas universitarias.
# Pila: ASP.NET Core MVC .NET 8 + Identidad + EF Core SQLite + Redis

# =========================
# PASOS PARA CORRER LOCALMENTE
# =========================

# Requisitos:
# - SDK de .NET 8
# - Git

# Instalación
git clone https://github.com/Leonardo213-pe/PortalAcademico.git
cd PortalAcademico/PortalAcademicoWeb
dotnet restore

# Migraciones
dotnet ef migrations add InitialCreate
dotnet ef database update

# Ejecutar proyecto
dotnet run

# Abrir en navegador:
# http://localhost:5247

# =========================
# VARIABLES DE ENTORNO
# =========================
# ASPNETCORE_ENVIRONMENT=Production
# ASPNETCORE_URLS=http://0.0.0.0:10000
# ConnectionStrings__DefaultConnection=Data Source=/data/portal_academico.db
# Redis__ConnectionString=redis://user:pass@host:port

# =========================
# CREDENCIALES INICIALES
# =========================
# Coordinador:
# coordinador@universidad.edu
# Password: Admin123!

# =========================
# RAMAS DEL PROYECTO
# =========================
# feature/bootstrap-dominio -> P1: Modelos y configuración base
# feature/catalogo-cursos   -> P2: Catálogo con filtros
# feature/matriculas        -> P3: Inscripciones y validaciones
# feature/sesion-redis      -> P4: Sesiones y caché Redis
# feature/panel-coordinador -> P5: Panel de coordinador
# deploy/render             -> P6: Despliegue y renderizado

# =========================
# URL EN PRODUCCIÓN
# =========================
# https://portalacademico-0zou.onrender.com
