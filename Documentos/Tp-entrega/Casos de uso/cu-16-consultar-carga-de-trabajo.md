# Caso de Uso: Consultar carga de trabajo

| Campo | Valor |
|-------|-------|
| ID | CU-16 |
| Nombre | Consultar carga de trabajo |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (organizar y priorizar sus tareas diarias), Supervisor (balanceo de carga) |
| Disparador (Trigger) | El técnico accede a la sección 'Carga de trabajo' desde el menú lateral |
| Prioridad / Frecuencia | Media / Diaria |
| Reglas de negocio relacionadas | RF-16 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte consultar la cantidad total de tickets asignados, clasificados por estado y prioridad, para gestionar su volumen de trabajo.

### 2. PRECONDICIONES
- El técnico debe haber iniciado sesión en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema identifica y autentica al técnico.
2. El sistema consulta los tickets asignados al técnico en la base de datos. [RF-16]
3. El sistema calcula la carga de trabajo y métricas correspondientes.
4. El sistema muestra la información estructurada en un panel de control.
5. El técnico consulta los resultados y lista de tareas pendientes.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. No posee tickets asignados**
1. El sistema no encuentra tickets pendientes o en proceso para el técnico.
2. El sistema informa: 'No posee tickets asignados en su carga de trabajo.'
3. El panel se muestra con indicadores en cero.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El técnico visualiza sus métricas de carga de trabajo actualizadas.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Carga de trabajo calculada y mostrada exitosamente |
| 401 | Unauthorized | Técnico no autenticado |
| 500 | Internal Server Error | Error interno del servidor al procesar las métricas |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-16-01 | TI-CU-16-01 |
| 2 | TU-CU-16-02 | TI-CU-16-02 |
| 3 | TU-CU-16-03 | TI-CU-16-03 |
| 4 | TU-CU-16-04 | TI-CU-16-04 |
| 5 | TU-CU-16-05 | TI-CU-16-05 |
| 2a-1 | TU-CU-16-A01 | TI-CU-16-A01 |
| 2a-2 | TU-CU-16-A02 | TI-CU-16-A02 |
| 2a-3 | TU-CU-16-A03 | TI-CU-16-A03 |
