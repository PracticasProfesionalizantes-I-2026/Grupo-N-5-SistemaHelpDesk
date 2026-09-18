# Caso de Uso: Recuperar contraseña

| Campo | Valor |
|-------|-------|
| ID | CU-03 |
| Nombre | Recuperar contraseña |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (restablecer acceso a su cuenta), Seguridad (garantizar autenticidad del solicitante) |
| Disparador (Trigger) | El usuario selecciona la opción '¿Olvidó su contraseña?' en la pantalla de inicio de sesión |
| Prioridad / Frecuencia | Media / Por demanda |
| Reglas de negocio relacionadas | RF-11, RN-Recuperacion-Segura |

### 1. BREVE DESCRIPCIÓN
Permite al usuario solicitar el restablecimiento de su contraseña mediante un código de verificación enviado a su correo electrónico registrado.

### 2. PRECONDICIONES
- El usuario debe tener una cuenta registrada en el sistema.
- El usuario debe tener acceso al correo electrónico asociado a su cuenta.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de solicitud de recuperación de contraseña.
2. El usuario ingresa su correo electrónico registrado.
3. El usuario confirma la solicitud presionando 'Enviar código'.
4. El sistema valida el formato del correo ingresado.
5. El sistema verifica la existencia de la cuenta en la base de datos.
6. El sistema genera un código de seguridad temporal y lo envía al correo del usuario.
7. El sistema muestra la pantalla para ingresar el código de verificación.
8. El usuario ingresa el código recibido y su nueva contraseña.
9. El sistema valida que el código sea correcto y no haya expirado.
10. El sistema valida que la nueva contraseña cumpla con las políticas de seguridad.
11. El sistema actualiza la contraseña en la base de datos de manera cifrada.
12. El sistema muestra un mensaje confirmando el cambio exitoso de contraseña.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Formato de correo inválido**
1. El sistema detecta un formato de correo electrónico incorrecto.
2. El sistema muestra el mensaje: 'Ingrese un correo electrónico válido.'
3. El flujo vuelve al paso 2 del flujo principal.

**5a. Cuenta no registrada**
1. El sistema verifica que el correo no pertenece a ningún usuario registrado.
2. El sistema muestra un mensaje informativo indicando que no se encontró una cuenta con ese correo.
3. El flujo se cancela y no se emite ningún código de seguridad.

**9a. Código de verificación incorrecto o expirado**
1. El sistema valida el código ingresado y detecta que es incorrecto o ha vencido.
2. El sistema muestra el mensaje: 'El código de verificación es inválido o ha expirado.'
3. El usuario puede reintentar el ingreso o solicitar un nuevo código.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La contraseña del usuario queda actualizada en el sistema.
- El usuario puede iniciar sesión utilizando su nueva contraseña.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Código enviado exitosamente / contraseña actualizada |
| 400 | Bad Request | Código inválido, expirado o contraseña no cumple requisitos (flujos 2a y 9a) |
| 404 | Not Found | Cuenta de correo no encontrada en el sistema (flujo 5a) |
| 500 | Internal Server Error | Fallo en el servicio de correo o base de datos |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-03-01 | TI-CU-03-01 |
| 2 | TU-CU-03-02 | TI-CU-03-02 |
| 3 | TU-CU-03-03 | TI-CU-03-03 |
| 4 | TU-CU-03-04 | TI-CU-03-04 |
| 5 | TU-CU-03-05 | TI-CU-03-05 |
| 6 | TU-CU-03-06 | TI-CU-03-06 |
| 7 | TU-CU-03-07 | TI-CU-03-07 |
| 8 | TU-CU-03-08 | TI-CU-03-08 |
| 9 | TU-CU-03-09 | TI-CU-03-09 |
| 10 | TU-CU-03-10 | TI-CU-03-10 |
| 11 | TU-CU-03-11 | TI-CU-03-11 |
| 12 | TU-CU-03-12 | TI-CU-03-12 |
| 2a-1 | TU-CU-03-A01 | TI-CU-03-A01 |
| 2a-2 | TU-CU-03-A02 | TI-CU-03-A02 |
| 2a-3 | TU-CU-03-A03 | TI-CU-03-A03 |
| 5a-1 | TU-CU-03-A04 | TI-CU-03-A04 |
| 5a-2 | TU-CU-03-A05 | TI-CU-03-A05 |
| 5a-3 | TU-CU-03-A06 | TI-CU-03-A06 |
| 9a-1 | TU-CU-03-A07 | TI-CU-03-A07 |
| 9a-2 | TU-CU-03-A08 | TI-CU-03-A08 |
| 9a-3 | TU-CU-03-A09 | TI-CU-03-A09 |
