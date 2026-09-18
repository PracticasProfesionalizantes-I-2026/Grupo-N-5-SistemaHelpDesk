# Caso de Uso: Ver soluciones adjuntadas previamente en cada ticket

| Campo | Valor |
|-------|-------|
| ID | CU-24 |
| Nombre | Ver soluciones adjuntadas previamente en cada ticket |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (consultar soluciones previas para agilizar la resolución de incidentes similares) |
| Disparador (Trigger) | El técnico abre un ticket y selecciona la opción de soluciones anteriores |
| Prioridad / Frecuencia | Media / Frecuente |
| Reglas de negocio relacionadas | RF-23 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte consultar el histórico de soluciones que han sido registradas y adjuntadas previamente en un ticket o tickets relacionados.

### 2. PRECONDICIONES
- El técnico debe haber iniciado sesión.
- El técnico debe tener acceso al ticket.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la vista detallada del ticket.
2. El técnico selecciona la opción 'Soluciones'. [RF-23]
3. El sistema busca las soluciones registradas asociadas al ticket.
4. El sistema muestra la lista de soluciones con descripción técnica y adjuntos.
5. El técnico consulta la información de las soluciones anteriores.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. No existen soluciones registradas**
1. El sistema verifica que no existen soluciones adjuntas para el ticket.
2. El sistema muestra el mensaje: 'No existen soluciones registradas.'
3. El técnico visualiza la pestaña vacía con opción de adjuntar una nueva solución.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El técnico puede consultar las soluciones técnicas anteriores del ticket.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Soluciones obtenidas y mostradas exitosamente |
| 401 | Unauthorized | Técnico no autenticado |
| 404 | Not Found | Ticket no encontrado |
| 500 | Internal Server Error | Error interno del servidor al consultar soluciones |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-24-01 | TI-CU-24-01 |
| 2 | TU-CU-24-02 | TI-CU-24-02 |
| 3 | TU-CU-24-03 | TI-CU-24-03 |
| 4 | TU-CU-24-04 | TI-CU-24-04 |
| 5 | TU-CU-24-05 | TI-CU-24-05 |
| 3a-1 | TU-CU-24-A01 | TI-CU-24-A01 |
| 3a-2 | TU-CU-24-A02 | TI-CU-24-A02 |
| 3a-3 | TU-CU-24-A03 | TI-CU-24-A03 |
