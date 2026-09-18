# AGENTS.md - Contexto Operativo para Agentes de IA

## 🎯 Propósito del Proyecto
Sistema HelpDesk API RESTful en .NET 10 con arquitectura N-Tier para gestión de tickets de soporte técnico con roles: Empleado, Técnico, Supervisor.

## 🛠️ Comandos CLI Esenciales

```bash
# Build completo
dotnet build

# Ejecutar API
dotnet run --project src/HelpDesk.API/HelpDesk.API.csproj

# Tests unitarios
dotnet test src/HelpDesk.Tests/HelpDesk.Tests.csproj

# Tests con cobertura
dotnet test src/HelpDesk.Tests/HelpDesk.Tests.csproj --collect:"XPlat Code Coverage"

# Migraciones EF Core
dotnet ef migrations add <Nombre> -p src/HelpDesk.DataAccess -s src/HelpDesk.API
dotnet ef database update -p src/HelpDesk.DataAccess -s src/HelpDesk.API

# Verificar formato
dotnet format
```

## 📐 Convención de Capas N-Tier

```
Controller (API)          → Validación básica DTO, Try/Catch mapea excepciones a HTTP
    ↓
Service (BusinessLogic)   → Reglas de negocio, validaciones complejas, MapToResponseDTO manual
    ↓
Repository (DataAccess)   → CRUD puro, AsNoTracking en reads, Guid en CreateAsync
    ↓
DbContext (EF Core)       → SaveChanges, Fluent API, DeleteBehavior.Restrict
    ↓
SQLite Database
```

### Responsabilidades por Capa

| Capa | Responsabilidad | Qué NO hacer |
|------|-----------------|--------------|
| **API/Controllers** | HTTP handling, DTO binding, Exception mapping | Lógica de negocio, validaciones complejas, LINQ-to-Entities |
| **BusinessLogic/Services** | Reglas de negocio, validaciones, mapeos DTO↔Entity | Acceso directo a BD, SQL, DbContext |
| **DataAccess/Repositories** | CRUD genérico, queries optimizadas | Reglas de negocio, validaciones |
| **Shared** | DTOs, Exceptions, Enums, Constants | Lógica de negocio, dependencias externas |

---

## 📋 Reglas de Negocio Implementadas

### Tickets
- **Creación**: Solo empleados crean (asignan su ID como EmpleadoId). Estado inicial = "Abierto".
- **Asignación**: Solo Supervisor. Técnico debe estar Activo y ser Rol=Tecnico. Cambia estado a "En Progreso".
- **Cambio Estado**: Técnico (solo sus tickets) o Supervisor. Genera `HistorialEstado` automático.
- **Reabrir**: Solo Supervisor. Ticket debe estar "Cerrado". Pasa a "Abierto".
- **Eliminar**: Solo Supervisor. Solo si estado = "Abierto".
- **SLA**: Crítica=4h, Alta=8h, Media=24h, Baja=72h. `estaVencido` calculado en DTO.

### Usuarios
- **Email único** en sistema.
- **Roles**: Empleado, Tecnico, Supervisor.
- **Desactivar**: Valida que no tenga tickets abiertos asignados.
- **Supervisor** no puede ser asignado como técnico.

### Categorías
- **Eliminar**: Valida que no tenga tickets asociados (DeleteBehavior.Restrict).

### Equipos (Teams)
- **Crear**: Requiere ≥1 técnico. Categoría obligatoria.
- **Técnicos**: Solo usuarios con Rol=Tecnico y Activo=true.

### Comentarios
- **Internos**: Solo Técnicos y Supervisores pueden crear/ver.
- **Ticket cerrado**: No permite agregar comentarios.

---

## 🗂️ Entidades Principales

| Entidad | Clave | Relaciones Clave |
|---------|-------|------------------|
| **User** (TPH: Empleado/Tecnico/Supervisor) | Guid Id | 1→N Ticket (Empleado/Tecnico), 1→N Comment, N↔M Team |
| **Ticket** | Guid Id | N→1 Priority, Status, Category, Empleado, Tecnico, Team; 1→N Comment, StatusHistory |
| **Category** | Guid Id | 1→N Ticket, 1→N Team |
| **Priority** | Guid Id | 1→N Ticket (SLAHoras, Nivel, Color) |
| **Status** | Guid Id | 1→N Ticket (EsFinal, Orden) |
| **Comment** | Guid Id | N→1 Ticket, User (EsInterno) |
| **StatusHistory** | Guid Id | N→1 Ticket, EstadoAnterior, EstadoNuevo, User |
| **Team** | Guid Id | N→1 Category, N↔M Tecnicos (User), 1→N Ticket |

---

## 🚨 Excepciones Tipadas y Códigos HTTP

| Excepción | HTTP | Uso |
|-----------|------|-----|
| `NotFoundException` | 404 | Entidad no existe |
| `ValidationException` | 400 | DTO inválido |
| `BusinessRuleException` | 409 | Violación regla de negocio |
| `DuplicateException` | 409 | Email duplicado, nombre duplicado |
| `DependencyException` | 409 | No se puede borrar (tiene dependencias) |
| `UnauthorizedActionException` | 403 | Rol sin permiso |
| `ConcurrencyException` | 409 | Conflicto concurrencia (RowVersion) |

**Mapeo en Controllers**: Try/Catch explícito → retorna HTTP correspondiente.

---

## 🧪 Testing

### Unit Tests (xUnit + Moub)
- Ubicación: `src/HelpDesk.Tests/UnitTests/`
- Servicios testeados: TicketService, UserService, CategoryService, TeamService
- Mocks: Todos los repositorios via interfaces

### Integration Tests (WebApplicationFactory)
- Ubicación: `src/HelpDesk.Tests/IntegrationTests/`
- Endpoints completos con BD en memoria

---

## 📝 Convenciones de Código

### DTOs (Shared/DTOs/)
- Nomenclatura: `<Entidad>CreateDTO`, `<Entidad>UpdateDTO`, `<Entidad>ResponseDTO`, `<Entidad>ListDTO`, `<Entidad>FilterDTO`
- Records inmutables
- Sin AutoMapper → `private MapToResponseDTO(entity)` en cada Service

### Entidades (DataAccess/Entities/)
- `Guid Id` asignado en `Repository.CreateAsync()`
- `byte[]? RowVersion` para concurrencia optimista
- Navegaciones `virtual` para EF Core
- Herencia TPH en User (Discriminator: "TipoUsuario")

### Repositorios (DataAccess/Repositories/)
- Genérico `Repository<T>` + interfaces específicas
- `AsNoTracking()` en **todas** las consultas de solo lectura
- `Include()` encadenados correctamente (reset query variable)
- Métodos `GetFilteredAsync` + `GetFilteredCountAsync` para paginación

### Servicios (BusinessLogic/Services/)
- Heredan de `BaseService` (validaciones comunes)
- Inyección de **todas** las interfaces de repositorio por constructor
- Métodos async con `await`
- Validaciones al inicio, lógica de negocio, mapeo al final

---

## 📦 Paquetes NuGet Clave

| Paquete | Uso |
|---------|-----|
| `Microsoft.EntityFrameworkCore.Sqlite` | ORM + SQLite |
| `Microsoft.EntityFrameworkCore.Design` | Migraciones (design-time) |
| `Microsoft.AspNetCore.OpenApi` | OpenAPI nativo .NET 10 |
| `Scalar.AspNetCore` | UI documentación |
| `Swashbuckle.AspNetCore` | Swagger/OpenAPI |
| `FluentValidation` | Validaciones DTO |
| `xunit` / `Moq` / `FluentAssertions` | Testing |

---

## 🔧 Configuración Importante

### DbContext (DataAccess/Data/HelpDeskDbContext.cs)
- `DeleteBehavior.Restrict` en todas las FK hijas
- Seed data en `OnModelCreating` (usuarios, categorías, prioridades, estados)
- TPH en User con Discriminator "TipoUsuario"

### DbInitializer (DataAccess/Data/DbInitializer.cs)
- `EnsureCreatedAsync()` + seed data al arrancar
- Se ejecuta en `Program.cs` via `scope.ServiceProvider`

### Program.cs (API/)
- DI: `AddScoped` para repositorios y servicios
- Middleware: `ExceptionHandlingMiddleware` (primero)
- CORS: AllowAnyOrigin
- Scalar UI en Development: `/scalar/v1`

---

## 🐛 Debugging Común

| Problema | Solución |
|----------|----------|
| "Castle.Core not found" en tests | Agregar `<PackageReference Include="Castle.Core" Version="5.1.1" />` en Tests.csproj |
| Error CS0266 en Includes | Resetear query variable: `query = query.Include(...)` |
| Migración falla | Verificar `DeleteBehavior.Restrict` en FKs |
| Tests no encuentran excepciones | Agregar `using HelpDesk.Shared.Exceptions;` en controllers/tests |

---

## 📌 Próximos Pasos / TODOs

- [ ] Implementar autenticación JWT real
- [ ] Agregar notificaciones (SignalR/Email)
- [ ] Tests de integración completos
- [ ] Dockerfile y docker-compose
- [ ] Health checks
- [ ] Rate limiting
- [ ] Auditoría completa (CreatedBy, UpdatedBy)