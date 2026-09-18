# Caso de Uso: Enviar comentarios en los tickets

| Campo | Valor |
|-------|-------|
| ID | CU-37 |
| Nombre | Enviar comentarios en los tickets |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (aportar directivas, instrucciones o notas de supervisión), Técnico y Usuario (recibir comunicación del supervisor) |
| Disparador (Trigger) | El supervisor selecciona 'Agregar comentario' dentro de un ticket |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-05 |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor agregar comentarios oficiales o notas de supervisión dentro de cualquier ticket del sistema.

### 2. PRECONDICIONES
- El supervisor debe haber iniciado sesión.
- El ticket debe existir en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la información del ticket.
2. El supervisor selecciona la opción de comentario.
3. El supervisor ingresa el contenido del comentario. [RF-05]
4. El supervisor confirma el envío.
5. El sistema valida el contenido ingresado.
6. El sistema registra el comentario en la base de datos. [RF-05]
7. El sistema actualiza el historial de comentarios del ticket.
8. El sistema notifica a los involucrados cuando corresponda.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. Comentario vacío**
1. El supervisor confirma el envío con el campo de comentario en blanco.
2. El sistema muestra un mensaje de error: 'Debe ingresar contenido en el comentario.'
3. El foco permanece en el área de texto.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El comentario del supervisor queda registrado en el ticket.
- Se actualiza el historial de eventos del ticket.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 201 | Created | Comentario de supervisor registrado exitosamente |
| 400 | Bad Request | Comentario vacío (flujo 3a) |
| 403 | Forbidden | Usuario sin permisos de supervisor |
| 404 | Not Found | Ticket no encontrado |
| 500 | Internal Server Error | Error interno del servidor al registrar el comentario |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-37-01 | TI-CU-37-01 |
| 2 | TU-CU-37-02 | TI-CU-37-02 |
| 3 | TU-CU-37-03 | TI-CU-37-03 |
| 4 | TU-CU-37-04 | TI-CU-37-04 |
| 5 | TU-CU-37-05 | TI-CU-37-05 |
| 6 | TU-CU-37-06 | TI-CU-37-06 |
| 7 | TU-CU-37-07 | TI-CU-37-07 |
| 8 | TU-CU-37-08 | TI-CU-37-08 |
| 3a-1 | TU-CU-37-A01 | TI-CU-37-A01 |
| 3a-2 | TU-CU-37-A02 | TI-CU-37-A02 |
| 3a-3 | TU-CU-37-A03 | TI-CU-37-A03 |
