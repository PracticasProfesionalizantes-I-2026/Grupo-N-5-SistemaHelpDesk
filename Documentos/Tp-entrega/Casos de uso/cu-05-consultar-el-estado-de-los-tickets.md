# Caso de Uso: Consultar el estado de los tickets

| Campo | Valor |
|-------|-------|
| ID | CU-05 |
| Nombre | Consultar el estado de los tickets |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (conocer el progreso y estado actual de sus solicitudes de soporte), Técnico de Soporte (mantener al usuario informado) |
| Disparador (Trigger) | El usuario selecciona la opción 'Mis Tickets' desde el menú principal |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-06, RN-Acceso-Tickets-Propios |

### 1. BREVE DESCRIPCIÓN
Permite al usuario final visualizar el listado de sus tickets y consultar en detalle el estado actual de cada uno sin modificar su información.

### 2. PRECONDICIONES
- El usuario debe estar registrado y haber iniciado sesión en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema consulta y muestra la lista de tickets asociados al usuario.
2. El usuario selecciona un ticket específico de la lista.
3. El sistema muestra la información detallada del ticket seleccionado.
4. El sistema muestra el estado actual y técnico asignado. [RF-06]
5. El usuario consulta la información del ticket.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**1a. No posee tickets registrados**
1. El sistema no encuentra ningún ticket asociado al usuario autenticado.
2. El sistema muestra el mensaje: 'No posee tickets registrados.'
3. El usuario visualiza una pantalla vacía con la opción de crear un nuevo ticket.

**2a. Ticket no encontrado**
1. El usuario intenta acceder a un ticket que no existe o fue eliminado.
2. El sistema muestra el mensaje de error: 'El ticket seleccionado no fue encontrado.'
3. El flujo regresa a la lista de tickets.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El usuario visualiza el estado y detalle de su ticket sin alterar sus datos.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Lista y detalle de tickets obtenidos exitosamente |
| 401 | Unauthorized | Usuario no autenticado |
| 403 | Forbidden | Intento de consultar tickets pertenecientes a otro usuario |
| 404 | Not Found | Ticket no encontrado (flujo 2a) |
| 500 | Internal Server Error | Error interno del servidor al consultar la base de datos |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-05-01 | TI-CU-05-01 |
| 2 | TU-CU-05-02 | TI-CU-05-02 |
| 3 | TU-CU-05-03 | TI-CU-05-03 |
| 4 | TU-CU-05-04 | TI-CU-05-04 |
| 5 | TU-CU-05-05 | TI-CU-05-05 |
| 1a-1 | TU-CU-05-A01 | TI-CU-05-A01 |
| 1a-2 | TU-CU-05-A02 | TI-CU-05-A02 |
| 1a-3 | TU-CU-05-A03 | TI-CU-05-A03 |
| 2a-1 | TU-CU-05-A04 | TI-CU-05-A04 |
| 2a-2 | TU-CU-05-A05 | TI-CU-05-A05 |
| 2a-3 | TU-CU-05-A06 | TI-CU-05-A06 |
