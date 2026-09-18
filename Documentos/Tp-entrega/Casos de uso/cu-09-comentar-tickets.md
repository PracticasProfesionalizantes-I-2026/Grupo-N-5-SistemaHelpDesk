# Caso de Uso: Comentar tickets

| Campo | Valor |
|-------|-------|
| ID | CU-09 |
| Nombre | Comentar tickets |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (aportar información adicional a su solicitud), Técnico de Soporte (recibir aclaraciones y comunicarse con el usuario) |
| Disparador (Trigger) | El usuario selecciona un ticket y presiona la opción 'Agregar comentario' |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-05, RF-10 |

### 1. BREVE DESCRIPCIÓN
Permite al usuario final registrar comentarios y aportar información adicional dentro del hilo de seguimiento de un ticket activo.

### 2. PRECONDICIONES
- El usuario debe haber iniciado sesión.
- El ticket debe pertenecer al usuario.
- El ticket no debe estar en estado cerrado definitivo.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el detalle del ticket y los comentarios anteriores.
2. El usuario selecciona la opción 'Agregar comentario'.
3. El sistema habilita el campo de texto para redactar el mensaje.
4. El usuario ingresa el contenido del comentario. [RF-05]
5. El usuario confirma el envío del comentario.
6. El sistema valida que el comentario no esté vacío.
7. El sistema registra el comentario asociado al ticket con fecha y hora. [RF-05]
8. El sistema actualiza la conversación del ticket en pantalla.
9. El sistema notifica al técnico asignado sobre el nuevo comentario recibido. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**4a. Comentario vacío**
1. El usuario intenta enviar un comentario sin contenido o con solo espacios.
2. El sistema muestra el mensaje de alerta: 'Debe ingresar un comentario válido antes de enviar.'
3. El flujo vuelve al paso 4 para que el usuario escriba su mensaje.

**6a. Ticket cerrado**
1. El sistema detecta que el ticket se encuentra cerrado.
2. El sistema muestra el mensaje: 'No es posible agregar comentarios en un ticket cerrado.'
3. El comentario no es registrado.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El comentario queda registrado y visible en el historial del ticket.
- Se notifica al técnico de soporte responsable.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 201 | Created | Comentario registrado exitosamente |
| 400 | Bad Request | Contenido de comentario vacío o inválido (flujo 4a) |
| 403 | Forbidden | Intento de comentar en ticket ajeno o cerrado (flujo 6a) |
| 404 | Not Found | Ticket no encontrado |
| 500 | Internal Server Error | Error interno del servidor al persistir el comentario |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-09-01 | TI-CU-09-01 |
| 2 | TU-CU-09-02 | TI-CU-09-02 |
| 3 | TU-CU-09-03 | TI-CU-09-03 |
| 4 | TU-CU-09-04 | TI-CU-09-04 |
| 5 | TU-CU-09-05 | TI-CU-09-05 |
| 6 | TU-CU-09-06 | TI-CU-09-06 |
| 7 | TU-CU-09-07 | TI-CU-09-07 |
| 8 | TU-CU-09-08 | TI-CU-09-08 |
| 9 | TU-CU-09-09 | TI-CU-09-09 |
| 4a-1 | TU-CU-09-A01 | TI-CU-09-A01 |
| 4a-2 | TU-CU-09-A02 | TI-CU-09-A02 |
| 4a-3 | TU-CU-09-A03 | TI-CU-09-A03 |
| 6a-1 | TU-CU-09-A04 | TI-CU-09-A04 |
| 6a-2 | TU-CU-09-A05 | TI-CU-09-A05 |
| 6a-3 | TU-CU-09-A06 | TI-CU-09-A06 |
