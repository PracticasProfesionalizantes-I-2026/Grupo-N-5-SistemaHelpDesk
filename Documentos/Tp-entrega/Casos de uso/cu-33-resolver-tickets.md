# Caso de Uso: Resolver tickets

| Campo | Valor |
|-------|-------|
| ID | CU-33 |
| Nombre | Resolver tickets |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (aprobar y cerrar tickets de forma definitiva), Usuario Final (confirmación de solución), Técnico de Soporte (cierre de caso) |
| Disparador (Trigger) | El supervisor selecciona la opción 'Resolver ticket' o 'Cerrar ticket' en el panel |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-06, RF-10, RF-12 |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor revisar la solución aplicada a un ticket y marcarlo formalmente como Resuelto o Cerrado en el sistema.

### 2. PRECONDICIONES
- El supervisor debe haber iniciado sesión.
- Debe existir el ticket en el sistema.
- El ticket debe encontrarse en un estado que permita su resolución.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra los tickets del sistema.
2. El supervisor selecciona un ticket.
3. El sistema muestra la información completa y soluciones propuestas.
4. El supervisor revisa la solución técnica registrada.
5. El supervisor selecciona la acción 'Resolver'.
6. El sistema valida la operación y el estado del ticket.
7. El sistema cambia el estado del ticket a 'Cerrado' o 'Resuelto'. [RF-06]
8. El sistema registra la fecha y hora exacta del cierre. [RF-12]
9. El sistema registra el cambio en el historial de auditoría.
10. El sistema notifica al usuario final sobre la resolución del ticket. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Ticket inexistente**
1. El sistema no encuentra el ticket seleccionado.
2. El sistema informa: 'El ticket no existe.'
3. El flujo finaliza regresando a la lista.

**6a. El ticket no cumple condiciones para resolverse**
1. El sistema detecta que el ticket no cumple las condiciones necesarias para ser resuelto.
2. El sistema informa: 'El ticket no cumple las condiciones para ser resuelto.'
3. La operación se cancela manteniendo el estado actual.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda en estado 'Cerrado'/'Resuelto'.
- Se registra la auditoría y fecha de resolución.
- El usuario recibe la notificación de cierre.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Información del ticket consultada correctamente |
| 204 | No Content | Ticket resuelto y cerrado exitosamente |
| 400 | Bad Request | El ticket no puede resolverse en su estado actual (flujo 6a) |
| 403 | Forbidden | Acceso restringido a Supervisores |
| 404 | Not Found | Ticket no encontrado (flujo 2a) |
| 500 | Internal Server Error | Error interno del servidor al cerrar el ticket |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-33-01 | TI-CU-33-01 |
| 2 | TU-CU-33-02 | TI-CU-33-02 |
| 3 | TU-CU-33-03 | TI-CU-33-03 |
| 4 | TU-CU-33-04 | TI-CU-33-04 |
| 5 | TU-CU-33-05 | TI-CU-33-05 |
| 6 | TU-CU-33-06 | TI-CU-33-06 |
| 7 | TU-CU-33-07 | TI-CU-33-07 |
| 8 | TU-CU-33-08 | TI-CU-33-08 |
| 9 | TU-CU-33-09 | TI-CU-33-09 |
| 10 | TU-CU-33-10 | TI-CU-33-10 |
| 2a-1 | TU-CU-33-A01 | TI-CU-33-A01 |
| 2a-2 | TU-CU-33-A02 | TI-CU-33-A02 |
| 2a-3 | TU-CU-33-A03 | TI-CU-33-A03 |
| 6a-1 | TU-CU-33-A04 | TI-CU-33-A04 |
| 6a-2 | TU-CU-33-A05 | TI-CU-33-A05 |
| 6a-3 | TU-CU-33-A06 | TI-CU-33-A06 |
