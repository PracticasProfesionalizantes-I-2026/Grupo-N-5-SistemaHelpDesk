namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record CategoryCreateDTO(
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    string? Nombre,

    [Required(ErrorMessage = "La descripción de la categoría es obligatoria")]
    [MinLength(5, ErrorMessage = "La descripción debe tener al menos 5 caracteres")]
    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
    string? Descripcion
);

public record CategoryUpdateDTO(
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    string? Nombre = null,

    [MinLength(5, ErrorMessage = "La descripción debe tener al menos 5 caracteres")]
    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
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