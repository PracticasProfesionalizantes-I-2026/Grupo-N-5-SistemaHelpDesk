# Caso de Uso: Consultar estadísticas

| Campo | Valor |
|-------|-------|
| ID | CU-30 |
| Nombre | Consultar estadísticas |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (evaluar el rendimiento global del soporte y SLA), Dirección (tomar decisiones estratégicas basadas en métricas) |
| Disparador (Trigger) | El supervisor accede al módulo de 'Estadísticas y Reportes' |
| Prioridad / Frecuencia | Media / Semanal |
| Reglas de negocio relacionadas | RF-08 |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor consultar reportes y estadísticas globales sobre tiempos de resolución, volumen de incidencias, desempeño por técnico y satisfacción.

### 2. PRECONDICIONES
- El supervisor debe haber iniciado sesión en el sistema.
- Deben existir datos de tickets registrados en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra las opciones y filtros de reportes estadísticos. [RF-08]
2. El supervisor selecciona los criterios (periodo de tiempo, equipo, categoría).
3. El sistema consulta los datos acumulados en la base de datos.
4. El sistema procesa la información y calcula los indicadores clave.
5. El sistema muestra las estadísticas en gráficos y tablas interactivas.
6. El supervisor analiza los resultados obtenidos.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. No existen datos suficientes**
1. El sistema no encuentra registros para el rango de fechas o filtros seleccionados.
2. El sistema informa: 'No hay datos suficientes para el periodo seleccionado.'
3. El supervisor puede ajustar el rango de búsqueda.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El supervisor visualiza las estadísticas y reportes consolidados del sistema.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Estadísticas y métricas generadas exitosamente |
| 400 | Bad Request | Rango de fechas o parámetros inválidos |
| 403 | Forbidden | Acceso restringido únicamente a Supervisores |
| 500 | Internal Server Error | Error interno del servidor al procesar estadísticas |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-30-01 | TI-CU-30-01 |
| 2 | TU-CU-30-02 | TI-CU-30-02 |
| 3 | TU-CU-30-03 | TI-CU-30-03 |
| 4 | TU-CU-30-04 | TI-CU-30-04 |
| 5 | TU-CU-30-05 | TI-CU-30-05 |
| 6 | TU-CU-30-06 | TI-CU-30-06 |
| 3a-1 | TU-CU-30-A01 | TI-CU-30-A01 |
| 3a-2 | TU-CU-30-A02 | TI-CU-30-A02 |
| 3a-3 | TU-CU-30-A03 | TI-CU-30-A03 |
