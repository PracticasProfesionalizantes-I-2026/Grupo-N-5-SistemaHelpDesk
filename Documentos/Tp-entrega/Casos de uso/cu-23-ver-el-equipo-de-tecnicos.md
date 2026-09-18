# Caso de Uso: Ver el equipo de técnicos

| Campo | Valor |
|-------|-------|
| ID | CU-23 |
| Nombre | Ver el equipo de técnicos |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (conocer a los integrantes de su equipo y su especialidad), Supervisor (organización de equipos) |
| Disparador (Trigger) | El técnico selecciona la opción 'Equipo de técnicos' en el menú |
| Prioridad / Frecuencia | Baja / Ocasional |
| Reglas de negocio relacionadas | RN-Permisos-Equipo, RF-20 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte consultar los integrantes, roles y datos de contacto del equipo de trabajo al cual pertenece.

### 2. PRECONDICIONES
- El técnico debe estar registrado y haber iniciado sesión.
- El técnico debe pertenecer a al menos un equipo de trabajo.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema identifica al técnico autenticado.
2. El sistema consulta el equipo al que pertenece en la base de datos.
3. El sistema obtiene los integrantes y la categoría de especialidad del equipo.
4. El sistema muestra la información completa del equipo en pantalla.
5. El técnico consulta la lista de integrantes y sus datos.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. No pertenece a ningún equipo**
1. El sistema detecta que el técnico aún no ha sido asignado a ningún equipo.
2. El sistema muestra el mensaje: 'No pertenece a ningún equipo.'
3. El flujo finaliza sin mostrar miembros.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El técnico visualiza la información de su equipo de trabajo.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Información del equipo obtenida correctamente |
| 401 | Unauthorized | Técnico no autenticado |
| 404 | Not Found | Equipo no encontrado (flujo 2a) |
| 500 | Internal Server Error | Error interno del servidor al consultar datos del equipo |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-23-01 | TI-CU-23-01 |
| 2 | TU-CU-23-02 | TI-CU-23-02 |
| 3 | TU-CU-23-03 | TI-CU-23-03 |
| 4 | TU-CU-23-04 | TI-CU-23-04 |
| 5 | TU-CU-23-05 | TI-CU-23-05 |
| 2a-1 | TU-CU-23-A01 | TI-CU-23-A01 |
| 2a-2 | TU-CU-23-A02 | TI-CU-23-A02 |
| 2a-3 | TU-CU-23-A03 | TI-CU-23-A03 |
