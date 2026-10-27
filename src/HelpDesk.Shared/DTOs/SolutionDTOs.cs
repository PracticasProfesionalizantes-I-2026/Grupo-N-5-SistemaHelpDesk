namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record SolutionCreateDTO(
    [Required(ErrorMessage = "La descripción de la solución es obligatoria")]
    [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
    [MaxLength(5000, ErrorMessage = "La descripción no puede superar los 5000 caracteres")]
    string? Descripcion,

    [MaxLength(255, ErrorMessage = "El nombre del archivo no puede superar los 255 caracteres")]
    string? NombreArchivo = null,

    [MaxLength(100, ErrorMessage = "El tipo de contenido no puede superar los 100 caracteres")]
    string? TipoContenido = null,

    [Range(1, 10485760, ErrorMessage = "El tamaño del archivo debe estar entre 1 byte y 10 MB (10485760 bytes)")]
    long? TamanioArchivo = null,

    byte[]? ContenidoArchivo = null
);

public record SolutionResponseDTO(
    Guid Id,
    Guid TicketId,
    Guid TecnicoId,
    string Description,
    string? FileName,
    string? FilePath,
    string? ContentType,
    long? FileSize,
    DateTime FechaCreacion
);

public record SolutionFormDTO(
    Guid TicketId,
    string[] ExtensionesPermitidas,
    long TamanoMaximoBytes,
    string TamanoMaximoTexto
);
