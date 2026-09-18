# Caso de Uso: Registrarse

| Campo | Valor |
|-------|-------|
| ID | CU-01 |
| Nombre | Registrarse |
| Actor Principal | Usuario Final |
| Alcance / Nivel | Usuario |
| Stakeholders e intereses | Usuario Final (crear cuenta para acceder al sistema de soporte), Supervisor (control de altas de usuarios), Sistema (garantizar unicidad de credenciales) |
| Disparador (Trigger) | El usuario selecciona la opción 'Registrarse' desde la pantalla de bienvenida o inicio de sesión |
| Prioridad / Frecuencia | Alta / Diaria |
| Reglas de negocio relacionadas | RF-21, RN-Email-Unico |

### 1. BREVE DESCRIPCIÓN
Permite a nuevos usuarios registrarse en la plataforma ingresando sus datos personales y credenciales para poder crear y gestionar tickets de soporte técnico.

### 2. PRECONDICIONES
- El usuario no debe tener una cuenta registrada previamente con el mismo correo electrónico.
- El usuario debe contar con un correo electrónico válido y accesible.

### 3. FLUJO PRINCIPAL (Camino Feliz)
1. El sistema muestra el formulario de registro de usuario.
2. El usuario ingresa su nombre completo, correo electrónico y contraseña. [RF-21]
3. El usuario confirma el registro presionando el botón 'Registrarse'.
4. El sistema valida los campos requeridos y el formato de los datos ingresados.
5. El sistema verifica que no exista una cuenta previa con el mismo correo electrónico. [RN-Email-Unico]
6. El sistema registra la cuenta del usuario en la base de datos con rol inicial de Empleado/Usuario Final.
7. El sistema muestra un mensaje confirmando el registro exitoso y redirige al inicio de sesión.

### 4. FLUJOS ALTERNATIVOS (Caminos Tristes / Excepciones)

**2a. Datos incompletos o formato inválido**
1. El sistema detecta campos vacíos o correo con formato incorrecto.
2. El sistema resalta los campos con error y muestra el mensaje: 'Debe completar todos los campos obligatorios con un formato válido.'
3. El usuario corrige los datos y el flujo retorna al paso 2 del flujo principal.

**5a. Cuenta ya existente**
1. El sistema identifica que el correo electrónico ya se encuentra registrado.
2. El sistema muestra el mensaje de error: 'Ya existe una cuenta registrada con este correo electrónico.'
3. El registro no se realiza y el sistema ofrece recuperar la contraseña o iniciar sesión.

### 5. SUB-VARIACIONES
- No se identifican sub-variaciones para este caso de uso.

### 6. POSTCONDICIONES
- La cuenta del usuario queda registrada en el sistema.
- El usuario queda habilitado para iniciar sesión y utilizar la plataforma.

---

### Anexos

#### Códigos HTTP utilizados
| Código | Significado | Uso en el CU |
|--------|-------------|--------------|
| 200 | OK | Formulario de registro cargado exitosamente |
| 201 | Created | Usuario registrado correctamente en el sistema |
| 400 | Bad Request | Campos obligatorios incompletos o formato inválido (flujo 2a) |
| 409 | Conflict | Correo electrónico ya registrado en el sistema (flujo 5a) |
| 500 | Internal Server Error | Error inesperado del servidor al persistir el usuario |

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
| 2a-1 | TU-CU-01-A01 | TI-CU-01-A01 |
| 2a-2 | TU-CU-01-A02 | TI-CU-01-A02 |
| 2a-3 | TU-CU-01-A03 | TI-CU-01-A03 |
| 5a-1 | TU-CU-01-A04 | TI-CU-01-A04 |
| 5a-2 | TU-CU-01-A05 | TI-CU-01-A05 |
| 5a-3 | TU-CU-01-A06 | TI-CU-01-A06 |
