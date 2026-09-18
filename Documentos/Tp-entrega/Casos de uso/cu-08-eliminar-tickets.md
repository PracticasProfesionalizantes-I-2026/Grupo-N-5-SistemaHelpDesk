# Caso de Uso: Eliminar tickets

| Campo | Valor |
|-------|-------|
| ID | CU-08 |
| Nombre | Eliminar tickets |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (cancelar solicitudes erróneas o innecesarias), Soporte Técnico (evitar atención de tickets descartados) |
| Disparador (Trigger) | El usuario selecciona la opción 'Eliminar' o 'Cancelar' dentro del detalle de su ticket |
| Prioridad / Frecuencia | Baja / Ocasional |
| Reglas de negocio relacionadas | RF-06, RN-Eliminacion-Tickets |

### 1. BREVE DESCRIPCIÓN
Permite al usuario cancelar o eliminar un ticket creado por error, siempre que el ticket se encuentre en estado inicial y no haya sido tomado en gestión.

### 2. PRECONDICIONES
- El usuario debe haber iniciado sesión.
- El ticket debe pertenecer al usuario.
- El ticket debe estar en estado inicial ('Abierto' / 'Creado sin Asignación').

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la lista de tickets del usuario.
2. El usuario selecciona un ticket en estado inicial.
3. El sistema muestra la información detallada del ticket.
4. El usuario selecciona la opción 'Eliminar Ticket'.
5. El sistema solicita confirmación mediante un cuadro de diálogo.
6. El usuario confirma la eliminación del ticket.
7. El sistema valida que el ticket cumpla las condiciones para ser cancelado. [RF-06]
8. El sistema actualiza el estado del ticket a 'Cancelado' o lo elimina del flujo activo. [RF-06]
9. El sistema registra la cancelación y auditoría de la operación.
10. El sistema muestra un mensaje confirmando la eliminación del ticket.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**6a. El usuario cancela la confirmación**
1. El usuario presiona 'Cancelar' en el diálogo de confirmación.
2. El sistema no realiza ninguna modificación en el ticket.
3. El flujo finaliza manteniendo el ticket activo.

**7a. El ticket no permite ser cancelado**
1. El sistema detecta que el ticket ya fue tomado por un técnico (estado 'En Proceso' o posterior).
2. El sistema muestra el mensaje de error: 'El ticket no puede eliminarse porque ya se encuentra en atención.'
3. La operación es cancelada y se conserva el estado del ticket.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda cancelado o eliminado del listado activo.
- Se registra la auditoría de la cancelación en el historial del sistema.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Operación de cancelación confirmada |
| 204 | No Content | Ticket eliminado correctamente |
| 400 | Bad Request | Petición inválida |
| 403 | Forbidden | El ticket no pertenece al usuario autenticado |
| 409 | Conflict | El ticket no puede ser eliminado por su estado actual (flujo 7a) |
| 500 | Internal Server Error | Error interno del servidor al procesar la cancelación |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-08-01 | TI-CU-08-01 |
| 2 | TU-CU-08-02 | TI-CU-08-02 |
| 3 | TU-CU-08-03 | TI-CU-08-03 |
| 4 | TU-CU-08-04 | TI-CU-08-04 |
| 5 | TU-CU-08-05 | TI-CU-08-05 |
| 6 | TU-CU-08-06 | TI-CU-08-06 |
| 7 | TU-CU-08-07 | TI-CU-08-07 |
| 8 | TU-CU-08-08 | TI-CU-08-08 |
| 9 | TU-CU-08-09 | TI-CU-08-09 |
| 10 | TU-CU-08-10 | TI-CU-08-10 |
| 6a-1 | TU-CU-08-A01 | TI-CU-08-A01 |
| 6a-2 | TU-CU-08-A02 | TI-CU-08-A02 |
| 6a-3 | TU-CU-08-A03 | TI-CU-08-A03 |
| 7a-1 | TU-CU-08-A04 | TI-CU-08-A04 |
| 7a-2 | TU-CU-08-A05 | TI-CU-08-A05 |
| 7a-3 | TU-CU-08-A06 | TI-CU-08-A06 |
