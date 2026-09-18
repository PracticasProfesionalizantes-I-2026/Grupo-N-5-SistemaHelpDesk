# Caso de Uso: Enviar mensajes

| Campo | Valor |
|-------|-------|
| ID | CU-10 |
| Nombre | Enviar mensajes |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (comunicarse directamente con el personal de soporte), Técnico de Soporte y Supervisor (atención ágil por mensajería interna) |
| Disparador (Trigger) | El usuario accede a la sección de mensajería y selecciona un destinatario |
| Prioridad / Frecuencia | Media / Frecuente |
| Reglas de negocio relacionadas | RF-18 |

### 1. BREVE DESCRIPCIÓN
Permite a los usuarios enviar y recibir mensajes directos a través del sistema de mensajería interna seleccionando un contacto habilitado.

### 2. PRECONDICIONES
- El usuario debe estar registrado y haber iniciado sesión.
- Debe existir un destinatario válido y habilitado en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la pantalla de mensajería con la lista de contactos y conversaciones activas. [RF-18]
2. El usuario selecciona un destinatario de la lista de contactos.
3. El sistema carga y muestra el historial de conversación con el contacto seleccionado.
4. El usuario escribe el texto del mensaje en el campo de entrada.
5. El usuario selecciona la opción 'Enviar'.
6. El sistema valida el contenido y formato del mensaje.
7. El sistema registra y despacha el mensaje hacia el destinatario.
8. El sistema actualiza la interfaz mostrando el nuevo mensaje en la conversación.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**4a. Mensaje vacío**
1. El usuario presiona 'Enviar' sin haber escrito ningún texto.
2. El sistema solicita ingresar contenido antes de realizar el envío.
3. El flujo vuelve al paso 4.

**6a. Destinatario no disponible**
1. El sistema detecta que el contacto de destino está inactivo o no habilitado para mensajería.
2. El sistema muestra el mensaje: 'El destinatario no se encuentra disponible.'
3. El mensaje no se envía.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El mensaje queda registrado en el historial de la conversación.
- El destinatario recibe el mensaje en su panel de mensajería.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Conversaciones y contactos cargados correctamente |
| 201 | Created | Mensaje enviado y registrado exitosamente |
| 400 | Bad Request | Contenido de mensaje vacío (flujo 4a) |
| 404 | Not Found | Destinatario no encontrado (flujo 6a) |
| 500 | Internal Server Error | Error interno del servidor en el servicio de mensajería |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-10-01 | TI-CU-10-01 |
| 2 | TU-CU-10-02 | TI-CU-10-02 |
| 3 | TU-CU-10-03 | TI-CU-10-03 |
| 4 | TU-CU-10-04 | TI-CU-10-04 |
| 5 | TU-CU-10-05 | TI-CU-10-05 |
| 6 | TU-CU-10-06 | TI-CU-10-06 |
| 7 | TU-CU-10-07 | TI-CU-10-07 |
| 8 | TU-CU-10-08 | TI-CU-10-08 |
| 4a-1 | TU-CU-10-A01 | TI-CU-10-A01 |
| 4a-2 | TU-CU-10-A02 | TI-CU-10-A02 |
| 4a-3 | TU-CU-10-A03 | TI-CU-10-A03 |
| 6a-1 | TU-CU-10-A04 | TI-CU-10-A04 |
| 6a-2 | TU-CU-10-A05 | TI-CU-10-A05 |
| 6a-3 | TU-CU-10-A06 | TI-CU-10-A06 |
