# Caso de Uso: Iniciar sesión

| Campo | Valor |
|-------|-------|
| ID | CU-02 |
| Nombre | Iniciar sesión |
| Actor Principal | Usuarios del sistema (Usuario Final, Técnico de Soporte, Supervisor) |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuarios del sistema (acceder a las funcionalidades según su rol), Sistema (validar identidad y proteger la seguridad de los recursos) |
| Disparador (Trigger) | El usuario ingresa sus credenciales en la pantalla de inicio de sesión y presiona 'Ingresar' |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-11 |

### 1. BREVE DESCRIPCIÓN
Permite a los usuarios autenticarse en el sistema mediante sus credenciales (email y contraseña) para acceder a las funcionalidades correspondientes a su rol.

### 2. PRECONDICIONES
- El usuario debe estar previamente registrado en el sistema.
- La cuenta del usuario debe encontrarse en estado activo.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de inicio de sesión.
2. El usuario ingresa su correo electrónico y contraseña. [RF-11]
3. El usuario confirma el ingreso presionando el botón 'Iniciar Sesión'.
4. El sistema valida el formato de las credenciales ingresadas.
5. El sistema verifica la autenticidad de las credenciales y el estado activo de la cuenta. [RF-11]
6. El sistema inicia la sesión y genera el token de autenticación.
7. El sistema redirige al usuario a la pantalla principal correspondiente a su rol.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**4a. Credenciales incorrectas**
1. El sistema verifica que la contraseña no coincide con el registro o el usuario no existe.
2. El sistema muestra el mensaje de error: 'Correo o contraseña incorrectos.'
3. El usuario permanece en la pantalla de inicio de sesión para reintentar.

**4b. Usuario inactivo o deshabilitado**
1. El sistema detecta que la cuenta del usuario se encuentra desactivada.
2. El sistema muestra el mensaje: 'Su cuenta se encuentra inactiva. Contacte al administrador.'
3. El acceso es denegado y no se genera la sesión.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- El usuario queda autenticado en la plataforma con una sesión válida.
- Se habilitan los módulos y permisos correspondientes al rol del usuario.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Autenticación exitosa y retorno de token de sesión |
| 400 | Bad Request | Credenciales con formato inválido o datos incompletos |
| 401 | Unauthorized | Credenciales incorrectas o cuenta inactiva (flujos 4a y 4b) |
| 500 | Internal Server Error | Error interno del servidor en el proceso de autenticación |

#### Matriz de trazabilidad
| Paso CU | Test Unitario | Test Integración |
|---------|---------------|------------------|
| 1 | TU-CU-02-01 | TI-CU-02-01 |
| 2 | TU-CU-02-02 | TI-CU-02-02 |
| 3 | TU-CU-02-03 | TI-CU-02-03 |
| 4 | TU-CU-02-04 | TI-CU-02-04 |
| 5 | TU-CU-02-05 | TI-CU-02-05 |
| 6 | TU-CU-02-06 | TI-CU-02-06 |
| 7 | TU-CU-02-07 | TI-CU-02-07 |
| 4a-1 | TU-CU-02-A01 | TI-CU-02-A01 |
| 4a-2 | TU-CU-02-A02 | TI-CU-02-A02 |
| 4a-3 | TU-CU-02-A03 | TI-CU-02-A03 |
| 4b-1 | TU-CU-02-A04 | TI-CU-02-A04 |
| 4b-2 | TU-CU-02-A05 | TI-CU-02-A05 |
| 4b-3 | TU-CU-02-A06 | TI-CU-02-A06 |
