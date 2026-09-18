# Caso de Uso: Crear tickets

| Campo | Valor |
|-------|-------|
| ID | CU-04 |
| Nombre | Crear tickets |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (reportar problemas para recibir asistencia), Técnico de Soporte (recibir asignaciones de tickets), Supervisor (monitorear incidencias creadas) |
| Disparador (Trigger) | El usuario presiona el botón 'Crear Ticket' desde la pantalla principal |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-01, RF-02, RF-06, RF-10 |

### 1. BREVE DESCRIPCIÓN
Permite a los usuarios del sistema generar un ticket de soporte técnico ingresando la descripción del incidente, categoría y urgencia, con asignación y notificación automática.

### 2. PRECONDICIONES
- El usuario debe estar registrado y con la sesión iniciada en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de creación de tickets.
2. El usuario ingresa la descripción detallada del problema. [RF-01]
3. El usuario selecciona la categoría correspondiente. [RF-01]
4. El usuario selecciona la urgencia del incidente. [RF-01]
5. El usuario confirma la creación del ticket.
6. El sistema valida que todos los campos obligatorios estén completos.
7. El sistema registra el nuevo ticket en la base de datos.
8. El sistema asigna el estado inicial 'Creado sin Asignación'. [RF-06]
9. El sistema asigna automáticamente el ticket a un técnico según la carga de trabajo y categoría. [RF-02]
10. El sistema redirige al usuario a la lista de sus tickets.
11. El sistema envía una notificación al usuario confirmando la creación del ticket. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Usuario no autenticado o sesión expirada**
1. El sistema detecta que la sesión del usuario ha caducado.
2. El sistema muestra el mensaje: 'Debe iniciar sesión para crear un ticket.'
3. El flujo redirige a la pantalla de inicio de sesión.

**6a. Campos obligatorios incompletos**
1. El usuario no completa la descripción, categoría o urgencia requerida.
2. El sistema informa los campos faltantes con mensajes de validación.
3. El usuario completa los campos y el flujo regresa al paso 5.

**7a. Error de conexión o fallo del sistema**
1. Al intentar registrar el ticket ocurre un error de comunicación con el servidor.
2. El sistema muestra el mensaje: 'Error al crear el ticket, intente nuevamente más tarde.'
3. El ticket no se registra y la transacción es revertida.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El ticket queda registrado en la base de datos con su estado inicial.
- El ticket queda asignado a un técnico disponible.
- Se emite la notificación correspondiente de creación.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Formulario y categorías cargadas correctamente |
| 201 | Created | Ticket creado y asignado exitosamente |
| 400 | Bad Request | Campos obligatorios incompletos o inválidos (flujo 6a) |
| 401 | Unauthorized | Sesión no válida o usuario no autenticado (flujo 2a) |
| 500 | Internal Server Error | Error interno del servidor al guardar el ticket (flujo 7a) |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-04-01 | TI-CU-04-01 |
| 2 | TU-CU-04-02 | TI-CU-04-02 |
| 3 | TU-CU-04-03 | TI-CU-04-03 |
| 4 | TU-CU-04-04 | TI-CU-04-04 |
| 5 | TU-CU-04-05 | TI-CU-04-05 |
| 6 | TU-CU-04-06 | TI-CU-04-06 |
| 7 | TU-CU-04-07 | TI-CU-04-07 |
| 8 | TU-CU-04-08 | TI-CU-04-08 |
| 9 | TU-CU-04-09 | TI-CU-04-09 |
| 10 | TU-CU-04-10 | TI-CU-04-10 |
| 11 | TU-CU-04-11 | TI-CU-04-11 |
| 2a-1 | TU-CU-04-A01 | TI-CU-04-A01 |
| 2a-2 | TU-CU-04-A02 | TI-CU-04-A02 |
| 2a-3 | TU-CU-04-A03 | TI-CU-04-A03 |
| 6a-1 | TU-CU-04-A04 | TI-CU-04-A04 |
| 6a-2 | TU-CU-04-A05 | TI-CU-04-A05 |
| 6a-3 | TU-CU-04-A06 | TI-CU-04-A06 |
| 7a-1 | TU-CU-04-A07 | TI-CU-04-A07 |
| 7a-2 | TU-CU-04-A08 | TI-CU-04-A08 |
| 7a-3 | TU-CU-04-A09 | TI-CU-04-A09 |
