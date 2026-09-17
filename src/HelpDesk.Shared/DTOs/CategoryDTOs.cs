namespace HelpDesk.Shared.DTOs;

public record CategoryCreateDTO(
    string Nombre,
    string Descripcion
);

public record CategoryUpdateDTO(
    string? Nombre = null,
    string? Descripcion = null,
    bool? Activo = null
);

public record CategoryResponseDTO(
    Guid Id,
    string Nombre,
    string Descripcion,
    bool Activo,
    int TicketsCount
);

public record CategoryListDTO(
    Guid Id,
    string Nombre,
    string Descripcion,
    bool Activo,
    int TicketsCount
);