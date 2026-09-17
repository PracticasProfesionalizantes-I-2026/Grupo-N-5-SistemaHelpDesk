namespace HelpDesk.Shared.DTOs;

public record UserCreateDTO(
    string Email,
    string NombreCompleto,
    string Rol
);

public record UserUpdateDTO(
    string? NombreCompleto = null,
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
    string Email
);

public record LoginResponseDTO(
    UserResponseDTO Usuario,
    string Token
);