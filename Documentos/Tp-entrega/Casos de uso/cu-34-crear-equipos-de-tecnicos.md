# Caso de Uso: Crear equipos de técnicos

| Campo | Valor |
|-------|-------|
| ID | CU-34 |
| Nombre | Crear equipos de técnicos |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (crear y organizar equipos de soporte), Técnico de Soporte (ser asignado a equipos), Sistema (usar equipos para asignación automática) |
| Disparador (Trigger) | El supervisor selecciona la opción 'Crear Equipo' desde el módulo de administración de equipos |
| Prioridad / Frecuencia | Media / Semanal |
| Reglas de negocio relacionadas | RF-02, RF-20 |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor crear equipos de técnicos para organizar la asignación de incidencias, definiendo un nombre, seleccionando los técnicos que lo integran y la categoría de especialización.

### 2. PRECONDICIONES
- El supervisor debe estar registrado.
- El supervisor debe haber iniciado sesión.
- Deben existir técnicos registrados en el sistema.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de creación de equipos.
2. El supervisor ingresa el nombre del equipo.
3. El supervisor selecciona uno o más técnicos para integrar el equipo.
4. El supervisor selecciona la categoría de especialización.
5. El supervisor confirma la creación del equipo.
6. El sistema valida los datos ingresados.
7. El sistema registra el nuevo equipo en la base de datos. [RF-20]
8. El sistema muestra un mensaje indicando que el equipo fue creado correctamente.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Nombre de equipo ya existente**
1. El supervisor ingresa un nombre que ya pertenece a otro equipo registrado.
2. El sistema muestra el mensaje: 'Ya existe un equipo con ese nombre.'
3. El flujo vuelve al paso 2 para corregir el nombre.

**6a. No se seleccionan técnicos**
1. El supervisor intenta crear un equipo sin seleccionar a ningún técnico.
2. El sistema muestra el mensaje: 'Debe seleccionar al menos un técnico.'
3. El flujo vuelve al paso 3.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El nuevo equipo queda registrado en el sistema.
- Los técnicos seleccionados quedan asociados al equipo creado.
- El equipo queda disponible para futuras asignaciones de tickets. [RF-02]

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Consulta exitosa de formulario y lista de técnicos |
| 201 | Created | Equipo creado correctamente |
| 400 | Bad Request | Nombre duplicado (flujo 2a) o sin técnicos seleccionados (flujo 6a) |
| 401 | Unauthorized | Supervisor no autenticado |
| 404 | Not Found | Técnicos o categoría no encontrados |
| 500 | Internal Server Error | Error del servidor al registrar el equipo |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-34-01 | TI-CU-34-01 |
| 2 | TU-CU-34-02 | TI-CU-34-02 |
| 3 | TU-CU-34-03 | TI-CU-34-03 |
| 4 | TU-CU-34-04 | TI-CU-34-04 |
| 5 | TU-CU-34-05 | TI-CU-34-05 |
| 6 | TU-CU-34-06 | TI-CU-34-06 |
| 7 | TU-CU-34-07 | TI-CU-34-07 |
| 8 | TU-CU-34-08 | TI-CU-34-08 |
| 2a-1 | TU-CU-34-A01 | TI-CU-34-A01 |
| 2a-2 | TU-CU-34-A02 | TI-CU-34-A02 |
| 2a-3 | TU-CU-34-A03 | TI-CU-34-A03 |
| 6a-1 | TU-CU-34-A04 | TI-CU-34-A04 |
| 6a-2 | TU-CU-34-A05 | TI-CU-34-A05 |
| 6a-3 | TU-CU-34-A06 | TI-CU-34-A06 |
