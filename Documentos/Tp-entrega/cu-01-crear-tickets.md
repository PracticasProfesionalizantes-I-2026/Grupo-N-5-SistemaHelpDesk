# Caso de Uso: Crear Tickets

| Campo | Valor |
|-------|-------|
| ID | CU-01 |
| Nombre | Crear Tickets |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (crear tickets para reportar problemas), Técnico de Soporte (recibir tickets asignados), Supervisor (visibilidad de carga de trabajo) |
| Disparador (Trigger) | El usuario presiona el botón "crear ticket" desde la pestaña principal |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-01, RF-02, RF-06, RF-10 |

### 1. BREVE DESCRIPCIÓN
Permite a los usuarios (Usuario Final, Técnico y Supervisor) del sistema crear tickets ingresando una descripción del problema, seleccionando la categoría y la urgencia.

### 2. PRECONDICIONES
- El Usuario debe estar registrado y con la cuenta iniciada.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de creación.
2. El usuario ingresa la descripción del problema. [RF-01]
3. El usuario selecciona la categoría. [RF-01]
4. El usuario selecciona la urgencia. [RF-01]
5. El usuario confirma la creación del ticket.
6. El sistema valida que todos los campos obligatorios estén completos.
7. El sistema registra el ticket.
8. El sistema asigna el estado inicial "Creado sin Asignación". [RF-06]
9. El sistema asigna automáticamente el ticket a un técnico según su carga de trabajo. [RF-02]
10. El sistema redirige al usuario a la lista de tickets.
11. El sistema envía una notificación al usuario confirmando la creación del ticket. [RF-10]

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Usuario no registrado o sesión expirada**
1. El usuario intenta acceder a la pestaña Crear Ticket sin haber iniciado sesión.
2. El sistema muestra un mensaje: "Debe iniciar sesión para crear un ticket".
3. El flujo vuelve al inicio de sesión.

**6a. Campos obligatorios incompletos**
1. El usuario no completa descripción, el sistema muestra "Falta descripción".
2. El usuario no completa categoría, el sistema muestra "Falta selección de categoría".
3. El usuario no completa urgencia, el sistema muestra "Falta asignar una urgencia".
4. El sistema muestra un mensaje de error: "Debe completar todos los campos requeridos".
5. El flujo regresa a la pantalla de creación de ticket para que el usuario corrija.

**7a. Problema de conexión o error del sistema**
1. Al intentar guardar el ticket, el sistema no responde (caída de red o servidor).
2. El sistema muestra: "Error al crear el ticket, intente nuevamente más tarde".
3. El ticket no se registra y el flujo termina en error.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- Asignar un estado al ticket.
- Asignar un técnico al ticket.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Consulta exitosa de formulario, validación exitosa |
| 201 | Created | Ticket creado correctamente |
| 400 | Bad Request | Campos obligatorios incompletos (flujo 6a) |
| 401 | Unauthorized | Usuario no autenticado / sesión expirada (flujo 2a) |
| 500 | Internal Server Error | Error del servidor al guardar (flujo 7a) |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-01-01 | TI-CU-01-01 |
| 2 | TU-CU-01-02 | TI-CU-01-02 |
| 3 | TU-CU-01-03 | TI-CU-01-03 |
| 4 | TU-CU-01-04 | TI-CU-01-04 |
| 5 | TU-CU-01-05 | TI-CU-01-05 |
| 6 | TU-CU-01-06 | TI-CU-01-06 |
| 7 | TU-CU-01-07 | TI-CU-01-07 |
| 8 | TU-CU-01-08 | TI-CU-01-08 |
| 9 | TU-CU-01-09 | TI-CU-01-09 |
| 10 | TU-CU-01-10 | TI-CU-01-10 |
| 11 | TU-CU-01-11 | TI-CU-01-11 |
| 2a-1 | TU-CU-01-A01 | TI-CU-01-A01 |
| 2a-2 | TU-CU-01-A02 | TI-CU-01-A02 |
| 2a-3 | TU-CU-01-A03 | TI-CU-01-A03 |
| 6a-1 | TU-CU-01-A04 | TI-CU-01-A04 |
| 6a-2 | TU-CU-01-A05 | TI-CU-01-A05 |
| 6a-3 | TU-CU-01-A06 | TI-CU-01-A06 |
| 6a-4 | TU-CU-01-A07 | TI-CU-01-A07 |
| 6a-5 | TU-CU-01-A08 | TI-CU-01-A08 |
| 7a-1 | TU-CU-01-A09 | TI-CU-01-A09 |
| 7a-2 | TU-CU-01-A10 | TI-CU-01-A10 |
| 7a-3 | TU-CU-01-A11 | TI-CU-01-A11 |