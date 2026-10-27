namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record UserCreateDTO(
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido (ej: usuario@dominio.com)")]
    [MaxLength(254, ErrorMessage = "El email no puede superar los 254 caracteres")]
    string? Email,

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
    string? NombreCompleto,

    [Required(ErrorMessage = "El rol es obligatorio. Valores válidos: Empleado, Tecnico, Supervisor")]
    [RegularExpression("^(Empleado|Tecnico|Supervisor)$",
        ErrorMessage = "El rol debe ser exactamente: Empleado, Tecnico o Supervisor")]
    string? Rol
);

public record UserUpdateDTO(
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
    string? NombreCompleto = null,

    [RegularExpression("^(Empleado|Tecnico|Supervisor)$",
        ErrorMessage = "El rol debe ser exactamente: Empleado, Tecnico o Supervisor")]
    string? Rol = null,

    bool? Activo = null
);

public record UserResponseDTO(
    Guid Id,
    string Email,
    string NombreCompleto,
    string Rol,
    bool Activo,
    DateTime FechaCreacion
);

public record UserListDTO(
    Guid Id,
    string Email,
    string NombreCompleto,
    string Rol,
    bool Activo,
    DateTime FechaCreacion,
    int TicketsAsignadosCount,
    int TicketsCreadosCount
);

public record UserFilterDTO(
    string? Rol = null,
    bool? Activo = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
);

public record LoginDTO(
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido (ej: usuario@dominio.com)")]
    [MaxLength(254, ErrorMessage = "El email no puede superar los 254 caracteres")]
    string? Email,

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(1, ErrorMessage = "La contraseña no puede estar vacía")]
    string? Password
);

public record LoginResponseDTO(
    UserResponseDTO Usuario,
    string Token
);

public record RegisterDTO(
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido (ej: usuario@dominio.com)")]
    [MaxLength(254, ErrorMessage = "El email no puede superar los 254 caracteres")]
    string? Email,

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
    string? NombreCompleto,

    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres")]
    string? Password = null
);