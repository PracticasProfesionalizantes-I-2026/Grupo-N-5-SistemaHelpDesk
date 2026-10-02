# Caso de Uso: Eliminar tickets

| Campo | Valor |
|-------|-------|
| ID | CU-08 |
| Nombre | Eliminar tickets |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (descartar tickets erróneos o innecesarios), Usuario Final (evitar que su solicitud quede registrada por error), Técnico de Soporte (no atender tickets descartados) |
| Disparador (Trigger) | El supervisor selecciona la opción 'Eliminar Ticket' desde la administración de tickets |
| Prioridad / Frecuencia | Baja / Ocasional |
| Reglas de negocio relacionadas | RF-06, RN-Eliminacion-Tickets |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor eliminar de forma definitiva un ticket del sistema, siempre que el ticket se encuentre en estado 'Abierto' y no haya sido tomado en gestión.

### 2. PRECONDICIONES
- El supervisor debe haber iniciado sesión con permisos de administración.
- El ticket debe existir en el sistema.
- El ticket debe encontrarse en estado 'Abierto'.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la lista de tickets del sistema.
2. El supervisor selecciona un ticket en estado 'Abierto'.
3. El sistema muestra la información detallada del ticket.
4. El supervisor selecciona la opción 'Eliminar Ticket'.
5. El sistema solicita confirmación mediante un cuadro de diálogo.
6. El supervisor confirma la eliminación del ticket.
7. El sistema valida que el actor posea rol de Supervisor y que el ticket se encuentre en estado 'Abierto'. [RF-06, RN-Eliminacion-Tickets]
8. El sistema elimina el ticket de forma definitiva de la base de datos.
9. El sistema actualiza el listado de tickets y muestra un mensaje confirmando la eliminación.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**6a. El supervisor cancela la confirmación**
1. El supervisor presiona 'Cancelar' en el diálogo de confirmación.
2. El sistema no realiza ninguna modificación en el ticket.
3. El flujo finaliza manteniendo el ticket activo.

**7a. El ticket no se encuentra en estado 'Abierto'**
1. El sistema detecta que el ticket se encuentra en un estado distinto de 'Abierto', por ejemplo 'En Proceso' o 'Resuelto'.
2. El sistema muestra el mensaje de error: 'Solo se pueden eliminar tickets en estado Abierto.'
3. La operación se cancela y se conserva el ticket con su estado actual.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda eliminado de forma definitiva del sistema y del listado activo.
- La operación solo se ejecuta cuando el ticket se encuentra en estado 'Abierto'.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Listado y detalle del ticket consultados correctamente |
| 204 | No Content | Ticket eliminado correctamente |
| 400 | Bad Request | Petición inválida |
| 403 | Forbidden | Acceso denegado: solo un Supervisor puede eliminar tickets |
| 404 | Not Found | El ticket no fue encontrado |
| 409 | Conflict | El ticket no puede eliminarse por su estado actual (flujo 7a) |
| 500 | Internal Server Error | Error interno del servidor al procesar la eliminación |

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
| 6a-1 | TU-CU-08-A01 | TI-CU-08-A01 |
| 6a-2 | TU-CU-08-A02 | TI-CU-08-A02 |
| 6a-3 | TU-CU-08-A03 | TI-CU-08-A03 |
| 7a-1 | TU-CU-08-A04 | TI-CU-08-A04 |
| 7a-2 | TU-CU-08-A05 | TI-CU-08-A05 |
| 7a-3 | TU-CU-08-A06 | TI-CU-08-A06 |
