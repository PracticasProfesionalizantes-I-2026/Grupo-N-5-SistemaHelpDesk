namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record TicketCreateDTO(
    [Required(ErrorMessage = "El título es obligatorio")]
    [MinLength(3, ErrorMessage = "El título debe tener al menos 3 caracteres")]
    [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres")]
    string? Titulo,

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
    [MaxLength(5000, ErrorMessage = "La descripción no puede superar los 5000 caracteres")]
    string? Descripcion,

    [Required(ErrorMessage = "Debe seleccionar una prioridad (PrioridadId: GUID)")]
    Guid? PrioridadId,

    [Required(ErrorMessage = "Debe seleccionar una categoría (CategoriaId: GUID)")]
    Guid? CategoriaId
);

public record TicketUpdateDTO(
    [MinLength(3, ErrorMessage = "El título debe tener al menos 3 caracteres")]
    [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres")]
    string? Titulo = null,

    [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
    [MaxLength(5000, ErrorMessage = "La descripción no puede superar los 5000 caracteres")]
    string? Descripcion = null,

    Guid? PrioridadId = null,
    Guid? CategoriaId = null
);

public record TicketAssignDTO(
    [Required(ErrorMessage = "El campo TecnicoId es obligatorio y debe ser un GUID válido")]
    Guid? TecnicoId
);

public record TicketStatusDTO(
    [Required(ErrorMessage = "El campo EstadoId es obligatorio y debe ser un GUID válido")]
    Guid? EstadoId,

    [MaxLength(1000, ErrorMessage = "La observación no puede superar los 1000 caracteres")]
    string? Observacion = null
);

public record TicketUpdateStatusDto
{
    public Guid TicketId { get; set; }
    public Guid EstadoId { get; set; }
}


public record TicketResponseDTO(
    Guid Id,
    string Titulo,
    string Descripcion,
    PriorityResponseDTO Prioridad,
    StatusResponseDTO Estado,
    CategoryResponseDTO Categoria,
    UserSummaryDTO Empleado,
    UserSummaryDTO? Tecnico,
    DateTime FechaCreacion,
    DateTime FechaActualizacion,
    DateTime? FechaResolucion,
    DateTime? FechaCierre,
    int SLAHoras,
    int ComentariosCount,
    bool EstaVencido
);

public record TicketListDTO(
    Guid Id,
    string Titulo,
    string Descripcion,
    PriorityResponseDTO Prioridad,
    StatusResponseDTO Estado,
    CategoryResponseDTO Categoria,
    UserSummaryDTO Empleado,
    UserSummaryDTO? Tecnico,
    DateTime FechaCreacion,
    DateTime FechaActualizacion,
    bool EstaVencido
);

public record PriorityResponseDTO(
    Guid Id,
    string Nombre,
    int Nivel,
    string Color,
    int SLAHoras
);

public record StatusResponseDTO(
    Guid Id,
    string Nombre,
    string Descripcion,
    bool EsFinal,
    int Orden
);

public record UserSummaryDTO(
    Guid Id,
    string NombreCompleto,
    string Email,
    string Rol
);

public record TicketSolutionDTO(
    Guid Id,
    Guid TicketId,
    string Descripcion,
    DateTime FechaCreacion,
    Guid AutorId,
    string AutorNombre
);

public record TicketFilterDTO(
    Guid? EstadoId = null,
    Guid? PrioridadId = null,
    Guid? CategoriaId = null,
    Guid? TecnicoId = null,
    Guid? EmpleadoId = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    int Page = 1,
    int PageSize = 20
);

public record PagedResultDTO<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record StatusHistoryResponseDTO(
    Guid Id,
    Guid TicketId,
    Guid? EstadoAnteriorId,
    StatusResponseDTO? EstadoAnterior,
    StatusResponseDTO EstadoNuevo,
    UserSummaryDTO Usuario,
    DateTime FechaCambio,
    string? Observacion
);