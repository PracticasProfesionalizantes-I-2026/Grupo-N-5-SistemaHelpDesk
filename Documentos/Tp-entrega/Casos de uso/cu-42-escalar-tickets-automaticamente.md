# Caso de Uso: Escalar tickets automáticamente

| Campo | Valor |
|-------|-------|
| ID | CU-42 |
| Nombre | Escalar tickets automáticamente |
| Actor Principal | Sistema |
| Alcance / Nivel | Sistema |
| Stakeholders e intereses | Supervisor (monitoreo y atención oportuna de tickets críticos), Usuario Final (garantía de cumplimiento de SLA) |
| Disparador (Trigger) | El proceso automático periódico detecta que un ticket cumple una condición de escalamiento de SLA |
| Prioridad / Frecuencia | Alta / Periódica |
| Reglas de negocio relacionadas | RF-06, RF-10, RN-SLA |

### 1. BREVE DESCRIPCIÓN
Permite al sistema escalar automáticamente aquellos tickets que superen el tiempo máximo de atención estipulado por las reglas de SLA.

### 2. PRECONDICIONES
- Debe existir un ticket activo en el sistema.
- El ticket debe superar el tiempo máximo de atención permitido según su prioridad.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema verifica periódicamente los tickets activos.
2. El sistema analiza el tiempo transcurrido y el estado de cada ticket.
3. El sistema identifica un ticket que supera el tiempo máximo de SLA. [RF-06]
4. El sistema cambia el estado del ticket a 'Escalado'. [RF-06]
5. El sistema registra el cambio en la base de datos y en el historial.
6. El sistema notifica al supervisor sobre el escalamiento del ticket. [RF-10]
7. El sistema registra la fecha y hora de la operación.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. No se cumple ninguna condición**
1. El sistema verifica que todos los tickets se encuentran dentro del SLA permitido.
2. El sistema mantiene el estado actual de los tickets.
3. El proceso finaliza sin realizar modificaciones.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda en estado 'Escalado' cuando corresponde.
- El cambio queda registrado en el historial y el supervisor es notificado.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Ciclo de verificación y escalamiento ejecutado correctamente |
| 500 | Internal Server Error | Error interno del servidor en el servicio en segundo plano |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-42-01 | TI-CU-42-01 |
| 2 | TU-CU-42-02 | TI-CU-42-02 |
| 3 | TU-CU-42-03 | TI-CU-42-03 |
| 4 | TU-CU-42-04 | TI-CU-42-04 |
| 5 | TU-CU-42-05 | TI-CU-42-05 |
| 6 | TU-CU-42-06 | TI-CU-42-06 |
| 7 | TU-CU-42-07 | TI-CU-42-07 |
| 3a-1 | TU-CU-42-A01 | TI-CU-42-A01 |
| 3a-2 | TU-CU-42-A02 | TI-CU-42-A02 |
| 3a-3 | TU-CU-42-A03 | TI-CU-42-A03 |
