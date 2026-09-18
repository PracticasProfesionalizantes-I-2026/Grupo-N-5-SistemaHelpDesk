# Caso de Uso: Adjuntar soluciones

| Campo | Valor |
|-------|-------|
| ID | CU-18 |
| Nombre | Adjuntar soluciones |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (documentar el procedimiento técnico de resolución), Usuario Final (recibir la solución al problema), Base de Conocimiento (reutilización de soluciones) |
| Disparador (Trigger) | El técnico selecciona 'Adjuntar solución' dentro de la vista del ticket |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-23 |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte registrar y adjuntar una solución formal detallada dentro de un ticket asignado para su posterior consulta y cierre.

### 2. PRECONDICIONES
- El técnico debe haber iniciado sesión.
- El técnico debe tener acceso al ticket asignado.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El técnico abre el ticket asignado.
2. El técnico selecciona la opción 'Adjuntar solución'.
3. El sistema muestra el formulario para registrar la solución.
4. El técnico ingresa la descripción técnica o adjunta archivos con la solución. [RF-23]
5. El técnico confirma la operación.
6. El sistema valida la información y formato de los archivos adjuntos.
7. El sistema registra la solución en la base de datos.
8. El sistema asocia la solución al ticket y actualiza el historial.
9. El sistema confirma la operación mostrando un mensaje de éxito.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**4a. Solución no válida o campos vacíos**
1. El sistema detecta que la descripción de la solución está vacía o el archivo adjunto excede el límite permitido.
2. El sistema informa el error específico al técnico.
3. El flujo vuelve al paso 4 para que el técnico corrija los datos.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La solución queda asociada al ticket y almacenada en la base de datos.
- El ticket queda listo para ser marcado como resuelto.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Formulario de solución cargado correctamente |
| 201 | Created | Solución adjuntada y registrada exitosamente |
| 400 | Bad Request | Campos vacíos o archivo adjunto no permitido (flujo 4a) |
| 403 | Forbidden | Técnico no autorizado para modificar este ticket |
| 404 | Not Found | Ticket no encontrado |
| 500 | Internal Server Error | Error interno del servidor al guardar la solución |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-18-01 | TI-CU-18-01 |
| 2 | TU-CU-18-02 | TI-CU-18-02 |
| 3 | TU-CU-18-03 | TI-CU-18-03 |
| 4 | TU-CU-18-04 | TI-CU-18-04 |
| 5 | TU-CU-18-05 | TI-CU-18-05 |
| 6 | TU-CU-18-06 | TI-CU-18-06 |
| 7 | TU-CU-18-07 | TI-CU-18-07 |
| 8 | TU-CU-18-08 | TI-CU-18-08 |
| 9 | TU-CU-18-09 | TI-CU-18-09 |
| 4a-1 | TU-CU-18-A01 | TI-CU-18-A01 |
| 4a-2 | TU-CU-18-A02 | TI-CU-18-A02 |
| 4a-3 | TU-CU-18-A03 | TI-CU-18-A03 |
