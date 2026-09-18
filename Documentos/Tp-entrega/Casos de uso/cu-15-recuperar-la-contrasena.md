# Caso de Uso: Recuperar la contraseña

| Campo | Valor |
|-------|-------|
| ID | CU-15 |
| Nombre | Recuperar la contraseña |
| Actor Principal | Técnico de Soporte |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Técnico de Soporte (recuperar acceso a sus herramientas de soporte), Seguridad (proteger cuentas de técnicos) |
| Disparador (Trigger) | El técnico selecciona 'Recuperar contraseña' desde la pantalla de inicio de sesión |
| Prioridad / Frecuencia | Media / Por demanda |
| Reglas de negocio relacionadas | RF-11, RN-Recuperacion-Segura |

### 1. BREVE DESCRIPCIÓN
Permite al técnico de soporte restablecer su contraseña de acceso mediante validación segura de su cuenta de correo corporativa.

### 2. PRECONDICIONES
- El técnico debe estar registrado en el sistema con rol de Técnico de Soporte.
- La cuenta debe estar activa.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de recuperación de contraseña para técnicos.
2. El técnico ingresa su correo electrónico corporativo registrado.
3. El sistema valida la existencia y rol de la cuenta en la base de datos.
4. El sistema inicia el procedimiento de recuperación y envía el token/código seguro al correo.
5. El técnico ingresa el código y establece su nueva contraseña cumpliendo las políticas de seguridad.
6. El sistema registra la nueva contraseña cifrada en la base de datos.
7. El sistema confirma la operación exitosa y redirige al inicio de sesión.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**3a. Cuenta no encontrada o inactiva**
1. El sistema verifica que el correo no corresponde a una cuenta activa de técnico.
2. El sistema informa que no se encontró una cuenta válida con los datos ingresados.
3. El flujo finaliza sin generar el código de recuperación.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La contraseña del técnico queda actualizada de forma segura.
- El técnico puede iniciar sesión con sus nuevas credenciales.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Código enviado / contraseña restablecida con éxito |
| 400 | Bad Request | Datos de formulario o código inválidos |
| 404 | Not Found | Cuenta no encontrada (flujo 3a) |
| 500 | Internal Server Error | Error interno del servidor al procesar la recuperación |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-15-01 | TI-CU-15-01 |
| 2 | TU-CU-15-02 | TI-CU-15-02 |
| 3 | TU-CU-15-03 | TI-CU-15-03 |
| 4 | TU-CU-15-04 | TI-CU-15-04 |
| 5 | TU-CU-15-05 | TI-CU-15-05 |
| 6 | TU-CU-15-06 | TI-CU-15-06 |
| 7 | TU-CU-15-07 | TI-CU-15-07 |
| 3a-1 | TU-CU-15-A01 | TI-CU-15-A01 |
| 3a-2 | TU-CU-15-A02 | TI-CU-15-A02 |
| 3a-3 | TU-CU-15-A03 | TI-CU-15-A03 |
