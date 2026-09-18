# Caso de Uso: Calificar la atención recibida

| Campo | Valor |
|-------|-------|
| ID | CU-07 |
| Nombre | Calificar la atención recibida |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (expresar su nivel de satisfacción con el servicio), Técnico de Soporte (recibir retroalimentación), Supervisor (medir calidad de atención y KPIs) |
| Disparador (Trigger) | El usuario presiona 'Calificar atención' sobre un ticket resuelto |
| Prioridad / Frecuencia | Media / Por evento |
| Reglas de negocio relacionadas | RF-08, RN-Calificacion-Resueltos |

### 1. BREVE DESCRIPCIÓN
Permite al usuario final calificar la calidad de atención brindada por el equipo técnico asignando una puntuación y un comentario opcional a un ticket resuelto.

### 2. PRECONDICIONES
- El usuario debe haber iniciado sesión.
- El ticket debe pertenecer al usuario.
- El ticket debe encontrarse en estado 'Resuelto' o 'Cerrado'.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de calificación del ticket.
2. El usuario selecciona una calificación numérica o estrellas de satisfacción.
3. El usuario ingresa un comentario opcional sobre la atención recibida.
4. El usuario confirma el envío de la calificación.
5. El sistema valida que la calificación se encuentre dentro del rango válido.
6. El sistema registra la calificación y el comentario asociados al ticket. [RF-08]
7. El sistema muestra un mensaje de confirmación y agradecimiento.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Calificación inválida o ausente**
1. El sistema detecta que no se seleccionó una puntuación válida.
2. El sistema muestra el mensaje: 'Debe seleccionar una calificación válida antes de enviar.'
3. El flujo vuelve al paso 2 para que el usuario seleccione una calificación.

**5a. Ticket no apto para calificar**
1. El sistema detecta que el ticket ya fue calificado previamente o no se encuentra resuelto.
2. El sistema muestra el mensaje: 'Este ticket no está disponible para ser calificado.'
3. La operación se cancela y no se registran cambios.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La calificación queda registrada y vinculada al ticket para métricas de calidad.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Formulario de calificación cargado exitosamente |
| 201 | Created | Calificación registrada correctamente |
| 400 | Bad Request | Puntuación inválida o datos incorrectos (flujo 2a) |
| 409 | Conflict | Ticket no resuelto o ya calificado con anterioridad (flujo 5a) |
| 500 | Internal Server Error | Error interno del servidor al guardar la calificación |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-07-01 | TI-CU-07-01 |
| 2 | TU-CU-07-02 | TI-CU-07-02 |
| 3 | TU-CU-07-03 | TI-CU-07-03 |
| 4 | TU-CU-07-04 | TI-CU-07-04 |
| 5 | TU-CU-07-05 | TI-CU-07-05 |
| 6 | TU-CU-07-06 | TI-CU-07-06 |
| 7 | TU-CU-07-07 | TI-CU-07-07 |
| 2a-1 | TU-CU-07-A01 | TI-CU-07-A01 |
| 2a-2 | TU-CU-07-A02 | TI-CU-07-A02 |
| 2a-3 | TU-CU-07-A03 | TI-CU-07-A03 |
| 5a-1 | TU-CU-07-A04 | TI-CU-07-A04 |
| 5a-2 | TU-CU-07-A05 | TI-CU-07-A05 |
| 5a-3 | TU-CU-07-A06 | TI-CU-07-A06 |
