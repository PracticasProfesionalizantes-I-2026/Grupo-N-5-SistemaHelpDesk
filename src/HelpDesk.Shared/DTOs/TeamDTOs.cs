namespace HelpDesk.Shared.DTOs;

public record TeamCreateDTO(
    string Nombre,
    string? Descripcion,
    Guid CategoriaId,
    IEnumerable<Guid> TecnicoIds
);

public record TeamUpdateDTO(
    string? Nombre = null,
    string? Descripcion = null,
    Guid? CategoriaId = null,
    bool? Activo = null,
    IEnumerable<Guid>? TecnicoIds = null
);

public record TeamResponseDTO(
    Guid Id,
    string Nombre,
    string? Descripcion,
    CategoryResponseDTO Categoria,
    IEnumerable<UserSummaryDTO> Tecnicos,
    DateTime FechaCreacion,
    bool Activo,
    int TicketsCount
);

public record TeamListDTO(
    Guid Id,
    string Nombre,
    string? Descripcion,
    CategoryResponseDTO Categoria,
    int TecnicosCount,
    DateTime FechaCreacion,
    bool Activo,
    int TicketsCount
);

public record TeamFilterDTO(
    Guid? CategoriaId = null,
    bool? Activo = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
);

public record TeamAddTecnicoDTO(
    Guid TecnicoId
);

public record TeamRemoveTecnicoDTO(
    Guid TecnicoId
);