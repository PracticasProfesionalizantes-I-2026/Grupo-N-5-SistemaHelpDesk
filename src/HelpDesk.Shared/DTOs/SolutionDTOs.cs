namespace HelpDesk.Shared.DTOs;

public record SolutionCreateDTO(
    string Descripcion,
    string? NombreArchivo = null,
    string? TipoContenido = null,
    long? TamanioArchivo = null,
    byte[]? ContenidoArchivo = null
);

public record SolutionResponseDTO(
    Guid Id,
    Guid TicketId,
    Guid TecnicoId,
    string Descripcion,
    string? NombreArchivo,
    string? TipoContenido,
    long? TamanioArchivo,
    DateTime FechaCreacion
);

public record SolutionFormDTO(
    Guid TicketId,
    string[] ExtensionesPermitidas,
    long TamanoMaximoBytes,
    string TamanoMaximoTexto
);