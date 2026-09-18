# HelpDesk API - Sistema de Gestión de Tickets

Sistema de HelpDesk desarrollado en **.NET 10** con arquitectura **N-Tier** (Clean Architecture) para la gestión de tickets de soporte técnico.

## 🏗️ Arquitectura N-Tier

```
src/
├── HelpDesk.API/              # Capa de Presentación (Controllers)
├── HelpDesk.BusinessLogic/    # Capa de Lógica de Negocio (Services)
├── HelpDesk.DataAccess/       # Capa de Acceso a Datos (Repositories + EF Core)
├── HelpDesk.Shared/           # Capa Compartida (DTOs, Exceptions, Enums, Constants)
└── HelpDesk.Tests/            # Tests Unitarios e Integración
```

### Flujo de Datos Obligatorio
```
Controller → Service → Repository → DbContext → SQLite
```

- **Inyección de dependencias** por constructor en todas las capas
- **Validaciones y reglas de negocio** exclusivamente en Services
- **Mapeos manuales** (sin AutoMapper) con métodos `MapToResponseDTO` en cada Service
- **Excepciones tipadas** mapeadas a códigos HTTP en Controllers

---

## 🚀 Inicio Rápido

### Prerrequisitos
- .NET 10 SDK
- SQLite (incluido en el paquete NuGet)

### Ejecución

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar la API
dotnet run --project src/HelpDesk.API/HelpDesk.API.csproj
```

La API estará disponible en:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Documentación Scalar**: `http://localhost:5000/scalar/v1`

### Base de Datos
Al iniciar la aplicación, se ejecuta automáticamente:
1. Migraciones de EF Core (`EnsureCreatedAsync`)
2. `DbInitializer` que carga datos de prueba:
   - 5 usuarios (1 Supervisor, 2 Empleados, 2 Técnicos)
   - 5 Categorías (Hardware, Software, Red, Accesos, Otro)
   - 4 Prioridades (Baja, Media, Alta, Crítica) con SLA
   - 5 Estados (Abierto, En Progreso, Resuelto, Cerrado, Reabierto)

---

## 📚 Catálogo de Endpoints

### Autenticación
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login simulado (retorna usuario + token mock) |

### Tickets
| Método | Endpoint | Roles | Descripción |
|--------|----------|-------|-------------|
| POST | `/api/tickets` | Todos | Crear ticket |
| GET | `/api/tickets` | Todos | Listar con filtros y paginación |
| GET | `/api/tickets/{id}` | Todos | Obtener detalle (valida permisos) |
| PUT | `/api/tickets/{id}` | Empleado (propio), Supervisor | Actualizar datos básicos |
| PATCH | `/api/tickets/{id}/assign` | Supervisor | Asignar técnico |
| PATCH | `/api/tickets/{id}/status` | Técnico (asignado), Supervisor | Cambiar estado |
| PATCH | `/api/tickets/{id}/reopen` | Supervisor | Reabrir ticket cerrado |
| DELETE | `/api/tickets/{id}` | Supervisor | Eliminar (solo estado Abierto) |

### Comentarios
| Método | Endpoint | Roles | Descripción |
|--------|----------|-------|-------------|
| POST | `/api/tickets/{ticketId}/comments` | Todos | Agregar comentario |
| GET | `/api/tickets/{ticketId}/comments` | Todos | Listar (internos solo Tec/Supervisor) |

### Usuarios
| Método | Endpoint | Roles | Descripción |
|--------|----------|-------|-------------|
| POST | `/api/users` | Supervisor | Crear usuario |
| GET | `/api/users` | Supervisor | Listar con filtros |
| GET | `/api/users/{id}` | Supervisor | Obtener usuario |
| PUT | `/api/users/{id}` | Supervisor | Actualizar usuario |
| PATCH | `/api/users/{id}/deactivate` | Supervisor | Desactivar (valida sin tickets abiertos) |

### Categorías
| Método | Endpoint | Roles | Descripción |
|--------|----------|-------|-------------|
| POST | `/api/categories` | Supervisor | Crear categoría |
| GET | `/api/categories` | Todos | Listar activas |
| GET | `/api/categories/{id}` | Todos | Obtener categoría |
| PUT | `/api/categories/{id}` | Supervisor | Actualizar |
| DELETE | `/api/categories/{id}` | Supervisor | Eliminar (valida sin tickets) |

### Equipos (Teams)
| Método | Endpoint | Roles | Descripción |
|--------|----------|-------|-------------|
| POST | `/api/teams` | Supervisor | Crear equipo |
| GET | `/api/teams` | Supervisor | Listar con filtros |
| GET | `/api/teams/{id}` | Supervisor | Obtener equipo |
| PUT | `/api/teams/{id}` | Supervisor | Actualizar |
| DELETE | `/api/teams/{id}` | Supervisor | Eliminar |
| POST | `/api/teams/{id}/tecnicos` | Supervisor | Agregar técnico |
| DELETE | `/api/teams/{id}/tecnicos/{tecnicoId}` | Supervisor | Quitar técnico |

### Reportes (Solo Supervisor)
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/reports/dashboard` | Estadísticas generales |
| GET | `/api/reports/tickets-by-status` | Tickets por estado |
| GET | `/api/reports/tickets-by-priority` | Tickets por prioridad |
| GET | `/api/reports/tickets-by-technician` | Tickets por técnico |
| GET | `/api/reports/sla-compliance` | Cumplimiento SLA |
| GET | `/api/reports/technician-workload` | Carga de trabajo técnicos |

### Catálogos (Solo Lectura)
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/priorities` | Listar prioridades |
| GET | `/api/status` | Listar estados |
| GET | `/api/status/initial` | Estado inicial |
| GET | `/api/status/closed` | Estado cerrado |

---

## 📋 Ejemplos Request/Response

### Crear Ticket
```bash
POST /api/tickets
Content-Type: application/json

{
  "titulo": "Problema con impresora",
  "descripcion": "La impresora no imprime documentos PDF",
  "prioridadId": "22222222-2222-2222-2222-222222222222",
  "categoriaId": "55555555-5555-5555-5555-555555555555"
}
```

**Response (201 Created):**
```json
{
  "id": "guid-generado",
  "titulo": "Problema con impresora",
  "descripcion": "La impresora no imprime documentos PDF",
  "prioridad": { "id": "...", "nombre": "Media", "nivel": 2, "color": "#CA8A04", "slaHoras": 24 },
  "estado": { "id": "...", "nombre": "Abierto", "descripcion": "Ticket recién creado", "esFinal": false, "orden": 1 },
  "categoria": { "id": "...", "nombre": "Hardware", "descripcion": "Problemas de hardware", "activo": true, "ticketsCount": 0 },
  "empleado": { "id": "...", "nombreCompleto": "Juan Pérez", "email": "juan.perez@empresa.com", "rol": "Empleado" },
  "tecnico": null,
  "fechaCreacion": "2026-09-17T10:00:00Z",
  "fechaActualizacion": "2026-09-17T10:00:00Z",
  "fechaResolucion": null,
  "fechaCierre": null,
  "slaHoras": 24,
  "comentariosCount": 0,
  "estaVencido": false
}
```

### Login
```bash
POST /api/auth/login
Content-Type: application/json

{ "email": "juan.perez@empresa.com" }
```

**Response (200 OK):**
```json
{
  "usuario": { ... },
  "token": "mock-token-guid-timestamp"
}
```

### Asignar Técnico (Solo Supervisor)
```bash
PATCH /api/tickets/{id}/assign
Content-Type: application/json

{ "tecnicoId": "cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa" }
```

### Cambiar Estado (Técnico asignado o Supervisor)
```bash
PATCH /api/tickets/{id}/status
Content-Type: application/json

{ "estadoId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "observacion": "En proceso" }
```

---

## 🧪 Tests

```bash
# Tests unitarios (xUnit + Moq)
dotnet test src/HelpDesk.Tests/HelpDesk.Tests.csproj
```

### Estructura de Tests
- **UnitTests/**: Tests de servicios con Moq (sin BD)
- **IntegrationTests/**: Tests de endpoints con WebApplicationFactory

---

## 📦 Colección Bruno

Archivos `.bru` en la carpeta `bruno/` para testing manual:
- `bruno/auth/` - Login
- `bruno/tickets/` - CRUD tickets, asignar, cambiar estado, reabrir
- `bruno/comments/` - Comentarios
- `bruno/users/` - Usuarios
- `bruno/categories/` - Categorías
- `bruno/teams/` - Equipos
- `bruno/reports/` - Reportes

Importar en [Bruno](https://www.usebruno.com/) → Open Collection → Seleccionar carpeta `bruno/`.

---

## 🔐 Roles y Permisos

| Acción | Empleado | Técnico | Supervisor |
|--------|----------|---------|------------|
| Crear ticket | ✅ | ✅ | ✅ |
| Ver tickets propios | ✅ | ✅ | ✅ |
| Ver todos los tickets | ❌ | ✅ (asignados) | ✅ |
| Asignar técnico | ❌ | ❌ | ✅ |
| Cambiar estado | ❌ | ✅ (sus tickets) | ✅ |
| Comentario público | ✅ | ✅ | ✅ |
| Comentario interno | ❌ | ✅ | ✅ |
| Gestionar usuarios | ❌ | ❌ | ✅ |
| Gestionar categorías | ❌ | ❌ | ✅ |
| Gestionar equipos | ❌ | ❌ | ✅ |
| Ver reportes | ❌ | ❌ | ✅ |
| Reabrir ticket | ❌ | ❌ | ✅ |

---

## ⚙️ Reglas de Negocio Principales

1. **Ticket cerrado** no permite modificaciones ni comentarios
2. **Solo Supervisor** puede asignar técnicos y reabrir tickets
3. **Técnico debe estar activo** y no puede ser Supervisor
4. **SLA por prioridad**: Crítica 4h, Alta 8h, Media 24h, Baja 72h
5. **Cambio de estado** genera automáticamente `HistorialEstado`
6. **Email único** en todo el sistema
7. **No se puede desactivar** usuario con tickets abiertos asignados
8. **Categoría/Equipo con tickets** no se puede eliminar

---

## 🛠️ Tecnologías

- **.NET 10** / ASP.NET Core
- **EF Core 9** + **SQLite**
- **Scalar** para documentación OpenAPI
- **xUnit + Moq** para testing
- **FluentValidation** para validaciones
- **Clean Architecture** (N-Tier)

---

## 📁 Estructura del Proyecto

```
Grupo-N-5-SistemaHelpDesk/
├── src/
│   ├── HelpDesk.API/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── HelpDesk.BusinessLogic/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   ├── HelpDesk.DataAccess/
│   │   ├── Data/
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Migrations/
│   ├── HelpDesk.Shared/
│   │   ├── DTOs/
│   │   ├── Exceptions/
│   │   ├── Enums/
│   │   └── Constants/
│   └── HelpDesk.Tests/
│       ├── UnitTests/
│       └── IntegrationTests/
├── bruno/
├── README.md
└── AGENTS.md
```