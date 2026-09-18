# Caso de Uso: Ver notificaciones

| Campo | Valor |
|-------|-------|
| ID | CU-11 |
| Nombre | Ver notificaciones |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (mantenerse al tanto de las novedades y cambios en sus tickets), Sistema (mantener trazabilidad de avisos) |
| Disparador (Trigger) | El usuario selecciona el icono de notificaciones en la barra superior |
| Prioridad / Frecuencia | Alta / Constante |
| Reglas de negocio relacionadas | RF-10, RN-Notificaciones-Propias |

### 1. BREVE DESCRIPCIÓN
Permite al usuario consultar las notificaciones automáticas generadas por el sistema respecto a cambios de estado, respuestas y asignaciones de sus tickets.

### 2. PRECONDICIONES
- El usuario debe haber iniciado sesión en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema consulta las notificaciones no leídas y recientes del usuario autenticado.
2. El sistema muestra la lista cronológica de notificaciones.
3. El usuario selecciona una notificación para ver su detalle.
4. El sistema muestra el contenido completo y redirige al ticket asociado.
5. El sistema marca la notificación como leída en la base de datos.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**1a. No existen notificaciones**
1. El sistema verifica que no existen notificaciones registradas para el usuario.
2. El sistema muestra el mensaje: 'No posee nuevas notificaciones.'
3. La lista se muestra vacía.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El usuario visualiza sus notificaciones y su estado pasa a leído.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Lista de notificaciones obtenida exitosamente |
| 204 | No Content | Notificación marcada como leída exitosamente |
| 401 | Unauthorized | Usuario no autenticado |
| 500 | Internal Server Error | Error interno del servidor al consultar notificaciones |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-11-01 | TI-CU-11-01 |
| 2 | TU-CU-11-02 | TI-CU-11-02 |
| 3 | TU-CU-11-03 | TI-CU-11-03 |
| 4 | TU-CU-11-04 | TI-CU-11-04 |
| 5 | TU-CU-11-05 | TI-CU-11-05 |
| 1a-1 | TU-CU-11-A01 | TI-CU-11-A01 |
| 1a-2 | TU-CU-11-A02 | TI-CU-11-A02 |
| 1a-3 | TU-CU-11-A03 | TI-CU-11-A03 |
