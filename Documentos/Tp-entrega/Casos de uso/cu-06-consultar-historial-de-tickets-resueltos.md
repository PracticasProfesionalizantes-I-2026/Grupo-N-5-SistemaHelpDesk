# Caso de Uso: Consultar historial de tickets resueltos

| Campo | Valor |
|-------|-------|
| ID | CU-06 |
| Nombre | Consultar historial de tickets resueltos |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (revisar soluciones a problemas anteriores y verificar respuestas), Soporte Técnico (historial de incidentes atendidos) |
| Disparador (Trigger) | El usuario selecciona la opción 'Resueltos' o 'Historial' desde el menú de tickets |
| Prioridad / Frecuencia | Media / Semanal |
| Reglas de negocio relacionadas | RF-06, RF-12, RN-Acceso-Tickets-Propios |

### 1. BREVE DESCRIPCIÓN
Permite al usuario final consultar el historial de todos los tickets que han sido resueltos o cerrados, visualizando las soluciones aplicadas y detalles históricos.

### 2. PRECONDICIONES
- El usuario debe estar registrado y haber iniciado sesión en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema verifica la sesión activa del usuario.
2. El usuario accede a la sección de tickets resueltos.
3. El sistema busca los tickets en estado 'Resuelto' o 'Cerrado' asociados al usuario. [RF-06]
4. El sistema muestra la lista cronológica de tickets resueltos.
5. El usuario selecciona un ticket resuelto para ver sus detalles.
6. El sistema muestra la información histórica completa, fecha de resolución y solución aplicada. [RF-12]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. No existen tickets resueltos**
1. El sistema no encuentra tickets en estado resuelto asociados al usuario.
2. El sistema muestra el mensaje informativo: 'No posee tickets resueltos en su historial.'
3. El usuario puede regresar al panel principal de tickets.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El usuario visualiza la información y solución histórica de sus tickets finalizados.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Historial de tickets resueltos obtenido exitosamente |
| 401 | Unauthorized | Usuario no autenticado |
| 500 | Internal Server Error | Error interno del servidor al consultar el historial |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-06-01 | TI-CU-06-01 |
| 2 | TU-CU-06-02 | TI-CU-06-02 |
| 3 | TU-CU-06-03 | TI-CU-06-03 |
| 4 | TU-CU-06-04 | TI-CU-06-04 |
| 5 | TU-CU-06-05 | TI-CU-06-05 |
| 6 | TU-CU-06-06 | TI-CU-06-06 |
| 3a-1 | TU-CU-06-A01 | TI-CU-06-A01 |
| 3a-2 | TU-CU-06-A02 | TI-CU-06-A02 |
| 3a-3 | TU-CU-06-A03 | TI-CU-06-A03 |
