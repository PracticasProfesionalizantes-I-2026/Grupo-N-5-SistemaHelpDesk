# Caso de Uso: Gestionar usuarios y roles

| Campo | Valor |
|-------|-------|
| ID | CU-31 |
| Nombre | Gestionar usuarios y roles |
| Actor Principal | Supervisor |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Supervisor (administrar cuentas, roles y accesos), Usuarios (contar con los permisos adecuados), Sistema (control RBAC) |
| Disparador (Trigger) | El supervisor accede al módulo de 'Gestión de Usuarios' |
| Prioridad / Frecuencia | Media / Semanal |
| Reglas de negocio relacionadas | RF-11, RN-Gestion-Usuarios |

### 1. BREVE DESCRIPCIÓN
Permite al supervisor administrar los usuarios registrados, modificar sus datos personales, asignar roles (Empleado, Técnico, Supervisor) y activar o desactivar cuentas.

### 2. PRECONDICIONES
- El supervisor debe haber iniciado sesión con permisos de administración.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra la lista completa de usuarios registrados.
2. El supervisor selecciona un usuario específico.
3. El sistema muestra la información detallada del usuario.
4. El supervisor modifica los datos, rol o estado del usuario. [RF-11]
5. El supervisor confirma los cambios.
6. El sistema valida la información y las reglas de negocio (ej. no desactivar técnicos con tickets activos).
7. El sistema guarda los cambios en la base de datos.
8. El sistema confirma la operación exitosa.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Usuario inexistente**
1. El sistema no encuentra el usuario solicitado.
2. El sistema informa: 'Usuario no encontrado.'
3. El flujo regresa a la lista de usuarios.

**6a. Datos inválidos o violación de regla**
1. El supervisor intenta una acción no permitida (ej. desactivar técnico con tickets en progreso).
2. El sistema muestra un mensaje de error explicando la restricción.
3. El flujo vuelve al paso 4 para corregir los datos.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La información del usuario, su rol o su estado quedan actualizados en la base de datos.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Lista y detalle de usuarios obtenidos exitosamente |
| 204 | No Content | Usuario o rol actualizado exitosamente |
| 400 | Bad Request | Datos de usuario inválidos |
| 403 | Forbidden | Acceso denegado a usuarios no supervisores |
| 404 | Not Found | Usuario no encontrado (flujo 2a) |
| 409 | Conflict | Conflicto por tickets activos asignados al desactivar (flujo 6a) |
| 500 | Internal Server Error | Error interno del servidor al actualizar usuario |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-31-01 | TI-CU-31-01 |
| 2 | TU-CU-31-02 | TI-CU-31-02 |
| 3 | TU-CU-31-03 | TI-CU-31-03 |
| 4 | TU-CU-31-04 | TI-CU-31-04 |
| 5 | TU-CU-31-05 | TI-CU-31-05 |
| 6 | TU-CU-31-06 | TI-CU-31-06 |
| 7 | TU-CU-31-07 | TI-CU-31-07 |
| 8 | TU-CU-31-08 | TI-CU-31-08 |
| 2a-1 | TU-CU-31-A01 | TI-CU-31-A01 |
| 2a-2 | TU-CU-31-A02 | TI-CU-31-A02 |
| 2a-3 | TU-CU-31-A03 | TI-CU-31-A03 |
| 6a-1 | TU-CU-31-A04 | TI-CU-31-A04 |
| 6a-2 | TU-CU-31-A05 | TI-CU-31-A05 |
| 6a-3 | TU-CU-31-A06 | TI-CU-31-A06 |
