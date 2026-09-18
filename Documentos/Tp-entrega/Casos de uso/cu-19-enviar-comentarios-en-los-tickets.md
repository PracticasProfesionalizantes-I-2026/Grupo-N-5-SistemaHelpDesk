# Caso de Uso: Enviar comentarios en los tickets

| Campo | Valor |
|-------|-------|
| ID | CU-19 |
| Nombre | Enviar comentarios en los tickets |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (comunicarse con el cliente o dejar notas técnicas), Usuario Final (recibir asistencia), Supervisor (auditoría) |
| Disparador (Trigger) | El técnico selecciona 'Agregar comentario' dentro del ticket |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-05 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte registrar comentarios públicos o notas técnicas internas dentro del hilo de seguimiento del ticket.

### 2. PRECONDICIONES
- El técnico debe haber iniciado sesión.
- Debe existir un ticket asignado al técnico.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el ticket y la conversación actual.
2. El técnico selecciona la opción de agregar comentario.
3. El técnico escribe el contenido del comentario. [RF-05]
4. El técnico confirma el envío del comentario.
5. El sistema valida el contenido ingresado.
6. El sistema registra el comentario en la base de datos con fecha y hora. [RF-05]
7. El sistema actualiza el historial y la conversación del ticket.
8. El sistema notifica al usuario cuando corresponda si el comentario es público.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. Comentario vacío**
1. El técnico intenta enviar un comentario sin texto.
2. El sistema solicita ingresar contenido antes de continuar.
3. El foco permanece en el campo de comentario.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El comentario queda registrado permanentemente en el ticket.
- Se notifica a los involucrados según la visibilidad del comentario.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 201 | Created | Comentario registrado exitosamente |
| 400 | Bad Request | Comentario vacío o inválido (flujo 3a) |
| 403 | Forbidden | Técnico sin permisos para comentar en el ticket |
| 404 | Not Found | Ticket no encontrado |
| 500 | Internal Server Error | Error interno del servidor al guardar el comentario |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-19-01 | TI-CU-19-01 |
| 2 | TU-CU-19-02 | TI-CU-19-02 |
| 3 | TU-CU-19-03 | TI-CU-19-03 |
| 4 | TU-CU-19-04 | TI-CU-19-04 |
| 5 | TU-CU-19-05 | TI-CU-19-05 |
| 6 | TU-CU-19-06 | TI-CU-19-06 |
| 7 | TU-CU-19-07 | TI-CU-19-07 |
| 8 | TU-CU-19-08 | TI-CU-19-08 |
| 3a-1 | TU-CU-19-A01 | TI-CU-19-A01 |
| 3a-2 | TU-CU-19-A02 | TI-CU-19-A02 |
| 3a-3 | TU-CU-19-A03 | TI-CU-19-A03 |
