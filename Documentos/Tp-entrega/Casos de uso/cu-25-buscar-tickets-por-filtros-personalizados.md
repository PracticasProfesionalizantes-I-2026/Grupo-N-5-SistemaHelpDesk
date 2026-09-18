# Caso de Uso: Buscar tickets por filtros personalizados

| Campo | Valor |
|-------|-------|
| ID | CU-25 |
| Nombre | Buscar tickets por filtros personalizados |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (encontrar incidentes específicos rápidamente para su atención oportuna) |
| Disparador (Trigger) | El técnico accede a la sección de búsqueda avanzada en su panel de gestión |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-09 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte filtrar y buscar tickets en el sistema utilizando criterios como estado, prioridad, rango de fechas y categoría.

### 2. PRECONDICIONES
- El técnico debe haber iniciado sesión en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra los filtros de búsqueda avanzada disponibles. [RF-09]
2. El técnico selecciona los criterios de filtrado (estado, prioridad, categoría, fecha).
3. El técnico ejecuta la búsqueda presionando 'Buscar'.
4. El sistema valida los filtros seleccionados.
5. El sistema busca en la base de datos los tickets que coinciden con los criterios.
6. El sistema muestra los resultados ordenados en una tabla interactiva.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**5a. Sin resultados coincidentes**
1. El sistema no encuentra tickets que coincidan con la combinación de filtros.
2. El sistema informa: 'No se encontraron tickets.'
3. El técnico puede modificar o limpiar los filtros de búsqueda.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El técnico visualiza la lista de tickets que coinciden con los filtros aplicados.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Búsqueda ejecutada y resultados devueltos exitosamente |
| 400 | Bad Request | Parámetros de búsqueda inválidos |
| 401 | Unauthorized | Técnico no autenticado |
| 500 | Internal Server Error | Error interno del servidor en la consulta de búsqueda |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-25-01 | TI-CU-25-01 |
| 2 | TU-CU-25-02 | TI-CU-25-02 |
| 3 | TU-CU-25-03 | TI-CU-25-03 |
| 4 | TU-CU-25-04 | TI-CU-25-04 |
| 5 | TU-CU-25-05 | TI-CU-25-05 |
| 6 | TU-CU-25-06 | TI-CU-25-06 |
| 5a-1 | TU-CU-25-A01 | TI-CU-25-A01 |
| 5a-2 | TU-CU-25-A02 | TI-CU-25-A02 |
| 5a-3 | TU-CU-25-A03 | TI-CU-25-A03 |
