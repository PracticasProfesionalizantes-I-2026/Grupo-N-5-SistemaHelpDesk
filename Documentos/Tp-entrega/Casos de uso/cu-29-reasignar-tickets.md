# Caso de Uso: Reasignar tickets

| Campo | Valor |
|-------|-------|
| ID | CU-29 |
| Nombre | Reasignar tickets |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (balancear la carga de trabajo y asegurar resolución), Técnico de Soporte (recibir nueva asignación), Usuario Final (continuidad de atención) |
| Disparador (Trigger) | El supervisor selecciona la opción 'Reasignar ticket' dentro de la administración de tickets |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-13, RF-02, RF-10, RF-12 |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor reasignar manualmente un ticket de un técnico a otro para balancear la carga de trabajo o resolver impedimentos.

### 2. PRECONDICIONES
- El supervisor debe haber iniciado sesión.
- Debe existir el ticket a reasignar.
- Deben existir técnicos disponibles y activos en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la lista de tickets.
2. El supervisor selecciona un ticket.
3. El sistema muestra la información del ticket y el responsable actual.
4. El supervisor selecciona la opción 'Reasignar'. [RF-13]
5. El sistema muestra la lista de técnicos disponibles con su carga de trabajo.
6. El supervisor selecciona el nuevo técnico responsable.
7. El supervisor confirma la reasignación.
8. El sistema valida la disponibilidad y rol del técnico seleccionado.
9. El sistema reasigna el ticket al nuevo técnico.
10. El sistema registra el cambio de asignación en el historial de auditoría. [RF-12]
11. El sistema notifica a los involucrados (técnico anterior, nuevo técnico y usuario). [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**5a. No existen técnicos disponibles**
1. El sistema verifica que no hay otros técnicos activos disponibles para recibir el ticket.
2. El sistema informa: 'No hay técnicos disponibles.'
3. La reasignación se cancela y se mantiene el técnico actual.

**8a. Error en la asignación**
1. Ocurre un error de persistencia o conflicto durante la reasignación.
2. El sistema informa que no fue posible reasignar el ticket.
3. Se conserva la asignación anterior sin cambios.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda formalmente asignado al nuevo técnico.
- El cambio queda registrado en el historial de eventos del ticket.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Técnicos disponibles listados correctamente |
| 204 | No Content | Ticket reasignado exitosamente |
| 400 | Bad Request | Técnico no válido o datos incorrectos |
| 403 | Forbidden | Usuario no posee rol de Supervisor |
| 404 | Not Found | Ticket o técnico no encontrado |
| 409 | Conflict | Error o conflicto de concurrencia al reasignar (flujo 8a) |
| 500 | Internal Server Error | Error interno del servidor al persistir reasignación |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-29-01 | TI-CU-29-01 |
| 2 | TU-CU-29-02 | TI-CU-29-02 |
| 3 | TU-CU-29-03 | TI-CU-29-03 |
| 4 | TU-CU-29-04 | TI-CU-29-04 |
| 5 | TU-CU-29-05 | TI-CU-29-05 |
| 6 | TU-CU-29-06 | TI-CU-29-06 |
| 7 | TU-CU-29-07 | TI-CU-29-07 |
| 8 | TU-CU-29-08 | TI-CU-29-08 |
| 9 | TU-CU-29-09 | TI-CU-29-09 |
| 10 | TU-CU-29-10 | TI-CU-29-10 |
| 11 | TU-CU-29-11 | TI-CU-29-11 |
| 5a-1 | TU-CU-29-A01 | TI-CU-29-A01 |
| 5a-2 | TU-CU-29-A02 | TI-CU-29-A02 |
| 5a-3 | TU-CU-29-A03 | TI-CU-29-A03 |
| 8a-1 | TU-CU-29-A04 | TI-CU-29-A04 |
| 8a-2 | TU-CU-29-A05 | TI-CU-29-A05 |
| 8a-3 | TU-CU-29-A06 | TI-CU-29-A06 |
