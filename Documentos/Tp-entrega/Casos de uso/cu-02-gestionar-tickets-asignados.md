# Caso de Uso: Gestionar Tickets Asignados

| Campo | Valor |
|-------|-------|
| ID | CU-02 |
| Nombre | Gestionar Tickets Asignados |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (gestionar sus tickets), Supervisor (monitorear gestión), Usuario Final (recibir notificaciones de actualizaciones) |
| Disparador (Trigger) | El técnico presiona la opción "Mis Tickets Asignados" desde la pantalla principal |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-05, RF-06, RF-07, RF-10, RF-12 |

### 1. BREVE DESCRIPCIÓN
Permite gestionar los tickets asignados, consultando la información del caso, agregando comentarios, adjuntando soluciones y actualizando su estado.

### 2. PRECONDICIONES
- El técnico debe estar registrado.
- El técnico debe haber iniciado sesión.
- El técnico debe tener al menos un ticket asignado.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la lista de tickets asignados.
2. El técnico selecciona un ticket.
3. El sistema muestra toda la información del ticket.
4. El técnico analiza el problema reportado.
5. El técnico agrega comentarios y/o adjunta una solución si corresponde. [RF-05]
6. El técnico actualiza el estado del ticket (En Proceso, Pendiente o Resuelto). [RF-06, RF-07]
7. El sistema guarda los cambios realizados.
8. El sistema registra la fecha y hora de la actualización. [RF-12]
9. El sistema notifica al usuario final sobre los cambios realizados en el ticket. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. El técnico no posee tickets asignados**
1. El sistema no encuentra tickets asignados al técnico.
2. El sistema muestra el mensaje: "No posee tickets asignados."

**7a. Error al guardar la información**
1. Al guardar los cambios ocurre un error de conexión o del servidor.
2. El sistema muestra el mensaje: "No fue posible guardar los cambios. Intente nuevamente."
3. El flujo vuelve al paso 5.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda actualizado con la información ingresada por el técnico.
- Se registra el historial de modificaciones realizadas.
- El usuario final recibe una notificación sobre la actualización del ticket.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Consulta exitosa de lista y detalle de tickets |
| 201 | Created | Comentario/solución agregada correctamente |
| 204 | No Content | Actualización de estado exitosa |
| 400 | Bad Request | Datos inválidos al actualizar |
| 401 | Unauthorized | Técnico no autenticado |
| 404 | Not Found | Ticket no encontrado |
| 409 | Conflict | Conflicto de concurrencia al guardar |
| 500 | Internal Server Error | Error del servidor al guardar (flujo 7a) |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-02-01 | TI-CU-02-01 |
| 2 | TU-CU-02-02 | TI-CU-02-02 |
| 3 | TU-CU-02-03 | TI-CU-02-03 |
| 4 | TU-CU-02-04 | TI-CU-02-04 |
| 5 | TU-CU-02-05 | TI-CU-02-05 |
| 6 | TU-CU-02-06 | TI-CU-02-06 |
| 7 | TU-CU-02-07 | TI-CU-02-07 |
| 8 | TU-CU-02-08 | TI-CU-02-08 |
| 9 | TU-CU-02-09 | TI-CU-02-09 |
| 2a-1 | TU-CU-02-A01 | TI-CU-02-A01 |
| 2a-2 | TU-CU-02-A02 | TI-CU-02-A02 |
| 7a-1 | TU-CU-02-A03 | TI-CU-02-A03 |
| 7a-2 | TU-CU-02-A04 | TI-CU-02-A04 |
| 7a-3 | TU-CU-02-A05 | TI-CU-02-A05 |