namespace HelpDesk.Shared.DTOs;

public record TicketCreateDTO(
    string Titulo,
    string Descripcion,
    Guid PrioridadId,
    Guid CategoriaId
);

public record TicketUpdateDTO(
    string? Titulo = null,
    string? Descripcion = null,
    Guid? PrioridadId = null,
    Guid? CategoriaId = null
);

public record TicketAssignDTO(
    Guid TecnicoId
);

public record TicketStatusDTO(
    Guid EstadoId,
    string? Observacion = null
);

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

public record CategoryResponseDTO(
    Guid Id,
    string Nombre,
    string Descripcion,
    bool Activo,
    int TicketsCount
);

public record UserSummaryDTO(
    Guid Id,
    string NombreCompleto,
    string Email,
    string Rol
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