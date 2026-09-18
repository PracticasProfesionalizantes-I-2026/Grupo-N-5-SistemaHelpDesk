# Caso de Uso: Buscar tickets por filtros personalizados

| Campo | Valor |
|-------|-------|
| ID | CU-12 |
| Nombre | Buscar tickets por filtros personalizados |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (localizar tickets rápidamente según criterios personalizados) |
| Disparador (Trigger) | El usuario accede al panel de búsqueda y filtros en su sección de tickets |
| Prioridad / Frecuencia | Media / Frecuente |
| Reglas de negocio relacionadas | RF-09, RN-Acceso-Tickets-Propios |

### 1. BREVE DESCRIPCIÓN
Permite al usuario final filtrar y buscar entre sus tickets utilizando criterios personalizados como estado, prioridad, fecha y palabras clave.

### 2. PRECONDICIONES
- El usuario debe haber iniciado sesión en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra los filtros disponibles para la búsqueda de tickets. [RF-09]
2. El usuario selecciona uno o más filtros (estado, prioridad, fecha).
3. El usuario ingresa los criterios o términos de búsqueda.
4. El usuario ejecuta la búsqueda presionando 'Filtrar'.
5. El sistema valida los criterios y parámetros ingresados.
6. El sistema consulta la base de datos filtrando únicamente los tickets pertenecientes al usuario.
7. El sistema muestra la lista de resultados coincidentes con información resumida.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**6a. Sin resultados coincidentes**
1. El sistema no encuentra ningún ticket que coincida con los filtros aplicados.
2. El sistema muestra el mensaje: 'No se encontraron tickets.'
3. El sistema ofrece la opción de restablecer los filtros.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El sistema muestra los tickets coincidentes con los filtros seleccionados.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Tickets filtrados obtenidos exitosamente |
| 400 | Bad Request | Criterios de filtro inválidos o mal formateados |
| 401 | Unauthorized | Usuario no autenticado |
| 500 | Internal Server Error | Error interno del servidor al realizar la búsqueda |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-12-01 | TI-CU-12-01 |
| 2 | TU-CU-12-02 | TI-CU-12-02 |
| 3 | TU-CU-12-03 | TI-CU-12-03 |
| 4 | TU-CU-12-04 | TI-CU-12-04 |
| 5 | TU-CU-12-05 | TI-CU-12-05 |
| 6 | TU-CU-12-06 | TI-CU-12-06 |
| 7 | TU-CU-12-07 | TI-CU-12-07 |
| 6a-1 | TU-CU-12-A01 | TI-CU-12-A01 |
| 6a-2 | TU-CU-12-A02 | TI-CU-12-A02 |
| 6a-3 | TU-CU-12-A03 | TI-CU-12-A03 |
