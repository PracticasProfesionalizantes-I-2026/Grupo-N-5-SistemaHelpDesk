# Caso de Uso: Enviar notificaciones

| Campo | Valor |
|-------|-------|
| ID | CU-43 |
| Nombre | Enviar notificaciones |
| Actor Principal | Sistema |
| Alcance / Nivel | Sistema |
| Stakeholders e intereses | Usuario Final, Técnico de Soporte, Supervisor (recibir información oportuna sobre el estado de los tickets) |
| Disparador (Trigger) | Se produce un evento notificable en el sistema (cambio de estado, asignación, comentario o resolución) |
| Prioridad / Frecuencia | Alta / Constante |
| Reglas de negocio relacionadas | RF-10 |

### 1. BREVE DESCRIPCIÓN
Permite al sistema enviar notificaciones automáticas a los usuarios involucrados cuando ocurre un evento relevante en un ticket.

### 2. PRECONDICIONES
- Debe ocurrir un evento notificable en el sistema.
- Debe existir al menos un destinatario válido asociado al evento.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema detecta el evento notificable.
2. El sistema identifica al destinatario correspondiente.
3. El sistema genera el contenido de la notificación. [RF-10]
4. El sistema envía la notificación a través de los canales configurados.
5. El sistema registra el envío en la base de datos.
6. El destinatario recibe la notificación en su panel.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Destinatario inexistente o inactivo**
1. El sistema detecta que el destinatario no existe o su cuenta está inactiva.
2. El sistema registra la advertencia en el log del sistema.
3. El envío se omite.

**4a. Error en el envío**
1. Ocurre una falla en el canal de comunicación o servicio de mensajería.
2. El sistema registra el error para su posterior reintento.
3. La notificación queda registrada como pendiente de entrega.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La notificación queda enviada o registrada como pendiente de reintento.
- Se registra la trazabilidad del envío en el sistema.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Notificación generada y despachada correctamente |
| 201 | Created | Notificación almacenada en la base de datos |
| 500 | Internal Server Error | Error interno del servidor en el servicio de notificaciones (flujo 4a) |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-43-01 | TI-CU-43-01 |
| 2 | TU-CU-43-02 | TI-CU-43-02 |
| 3 | TU-CU-43-03 | TI-CU-43-03 |
| 4 | TU-CU-43-04 | TI-CU-43-04 |
| 5 | TU-CU-43-05 | TI-CU-43-05 |
| 6 | TU-CU-43-06 | TI-CU-43-06 |
| 2a-1 | TU-CU-43-A01 | TI-CU-43-A01 |
| 2a-2 | TU-CU-43-A02 | TI-CU-43-A02 |
| 2a-3 | TU-CU-43-A03 | TI-CU-43-A03 |
| 4a-1 | TU-CU-43-A04 | TI-CU-43-A04 |
| 4a-2 | TU-CU-43-A05 | TI-CU-43-A05 |
| 4a-3 | TU-CU-43-A06 | TI-CU-43-A06 |
