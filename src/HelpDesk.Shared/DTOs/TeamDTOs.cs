namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record TeamCreateDTO(
    [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    string? Nombre,

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
    string? Descripcion,

    [Required(ErrorMessage = "La categoría es obligatoria (CategoriaId: GUID)")]
    Guid CategoriaId,

    [Required(ErrorMessage = "Debe especificar al menos un técnico (TecnicoIds: array de GUIDs)")]
    IEnumerable<Guid>? TecnicoIds
);

public record TeamUpdateDTO(
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    string? Nombre = null,

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
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
    [Required(ErrorMessage = "El campo TecnicoId es obligatorio y debe ser un GUID válido")]
    Guid TecnicoId
);

public record TeamRemoveTecnicoDTO(
    [Required(ErrorMessage = "El campo TecnicoId es obligatorio y debe ser un GUID válido")]
    Guid TecnicoId
);