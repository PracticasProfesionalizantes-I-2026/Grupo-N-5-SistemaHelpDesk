# Caso de Uso: Asignar tickets automáticamente

| Campo | Valor |
|-------|-------|
| ID | CU-41 |
| Nombre | Asignar tickets automáticamente |
| Actor Principal | Sistema |
| Alcance / Nivel | Sistema |
| Stakeholders e intereses | Sistema (optimizar la distribución de trabajo), Técnico de Soporte (recibir asignaciones equilibradas), Supervisor (control de asignación) |
| Disparador (Trigger) | Se crea un nuevo ticket que requiere asignación en el sistema |
| Prioridad / Frecuencia | Alta / Constante |
| Reglas de negocio relacionadas | RF-02, RF-06, RF-10 |

### 1. BREVE DESCRIPCIÓN
Permite al sistema asignar automáticamente los tickets recién creados a los técnicos disponibles considerando su carga de trabajo y especialidad.

### 2. PRECONDICIONES
- Debe existir un ticket sin técnico asignado.
- Deben existir técnicos disponibles en el sistema.
- El sistema debe contar con información sobre la carga de trabajo de los técnicos.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema detecta un ticket sin asignación.
2. El sistema consulta los técnicos disponibles pertenecientes a la categoría del ticket.
3. El sistema obtiene la carga de trabajo de cada técnico. [RF-02]
4. El sistema analiza la información y selecciona al técnico con menor carga.
5. El sistema selecciona al técnico correspondiente.
6. El sistema asigna el ticket al técnico seleccionado.
7. El sistema registra la asignación en la base de datos.
8. El sistema actualiza el estado correspondiente del ticket. [RF-06]
9. El sistema notifica la asignación al técnico y al usuario. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. No existen técnicos disponibles**
1. El sistema verifica que no hay técnicos activos en la categoría requerida.
2. El ticket permanece en estado 'Creado sin Asignación' o pendiente.
3. El sistema genera una alerta para la intervención del supervisor.

**4a. Información insuficiente o empate de carga**
1. El sistema detecta empate de carga; aplica criterio de asignación por turnos (round-robin).
2. Si no es posible resolver la asignación, el ticket queda pendiente.
3. El sistema registra la incidencia en el log de eventos.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda formalmente asignado cuando existe disponibilidad.
- La asignación y fecha quedan registradas en el historial.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Proceso de asignación automática completado exitosamente |
| 500 | Internal Server Error | Error interno del servidor durante la ejecución del proceso batch |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-41-01 | TI-CU-41-01 |
| 2 | TU-CU-41-02 | TI-CU-41-02 |
| 3 | TU-CU-41-03 | TI-CU-41-03 |
| 4 | TU-CU-41-04 | TI-CU-41-04 |
| 5 | TU-CU-41-05 | TI-CU-41-05 |
| 6 | TU-CU-41-06 | TI-CU-41-06 |
| 7 | TU-CU-41-07 | TI-CU-41-07 |
| 8 | TU-CU-41-08 | TI-CU-41-08 |
| 9 | TU-CU-41-09 | TI-CU-41-09 |
| 2a-1 | TU-CU-41-A01 | TI-CU-41-A01 |
| 2a-2 | TU-CU-41-A02 | TI-CU-41-A02 |
| 2a-3 | TU-CU-41-A03 | TI-CU-41-A03 |
| 4a-1 | TU-CU-41-A04 | TI-CU-41-A04 |
| 4a-2 | TU-CU-41-A05 | TI-CU-41-A05 |
| 4a-3 | TU-CU-41-A06 | TI-CU-41-A06 |
