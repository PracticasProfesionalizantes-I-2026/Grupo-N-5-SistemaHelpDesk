# Caso de Uso: Actualizar el estado de los tickets

| Campo | Valor |
|-------|-------|
| ID | CU-17 |
| Nombre | Actualizar el estado de los tickets |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (reflejar avance del ticket), Supervisor (monitorear avance), Usuario Final (recibir aviso del nuevo estado) |
| Disparador (Trigger) | El técnico selecciona la opción para actualizar el estado de un ticket |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-06, RF-07, RF-10, RF-12 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico modificar el estado de un ticket asignado respetando el flujo de estados y las reglas de negocio del sistema.

### 2. PRECONDICIONES
- El técnico debe haber iniciado sesión.
- El ticket debe estar asignado al técnico.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra los tickets asignados al técnico.
2. El técnico selecciona un ticket.
3. El sistema muestra la información y opciones de estado disponibles.
4. El técnico selecciona el nuevo estado (ej. 'En Proceso', 'En Espera', 'Resuelto'). [RF-07]
5. El sistema valida que la transición de estado cumpla las reglas de negocio. [RF-06]
6. El sistema actualiza el estado del ticket en la base de datos.
7. El sistema registra la fecha y hora de la actualización. [RF-12]
8. El sistema registra el cambio en el historial de estados. [RF-12]
9. El sistema notifica al usuario final sobre el cambio de estado. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**5a. Estado no permitido**
1. El sistema detecta una transición de estado no válida según las reglas.
2. El sistema rechaza el cambio y muestra un mensaje de error: 'Transición de estado no permitida.'
3. El estado del ticket permanece sin cambios.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda con el nuevo estado actualizado.
- El cambio queda registrado en el historial de auditoría.
- El usuario recibe la notificación correspondiente.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Consulta de opciones de estado exitosa |
| 204 | No Content | Estado del ticket actualizado exitosamente |
| 400 | Bad Request | Estado o transición no permitida (flujo 5a) |
| 401 | Unauthorized | Técnico no autenticado |
| 403 | Forbidden | El ticket no se encuentra asignado a este técnico |
| 404 | Not Found | Ticket no encontrado |
| 500 | Internal Server Error | Error interno del servidor al actualizar estado |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-17-01 | TI-CU-17-01 |
| 2 | TU-CU-17-02 | TI-CU-17-02 |
| 3 | TU-CU-17-03 | TI-CU-17-03 |
| 4 | TU-CU-17-04 | TI-CU-17-04 |
| 5 | TU-CU-17-05 | TI-CU-17-05 |
| 6 | TU-CU-17-06 | TI-CU-17-06 |
| 7 | TU-CU-17-07 | TI-CU-17-07 |
| 8 | TU-CU-17-08 | TI-CU-17-08 |
| 9 | TU-CU-17-09 | TI-CU-17-09 |
| 5a-1 | TU-CU-17-A01 | TI-CU-17-A01 |
| 5a-2 | TU-CU-17-A02 | TI-CU-17-A02 |
| 5a-3 | TU-CU-17-A03 | TI-CU-17-A03 |
