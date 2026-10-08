namespace HelpDesk.Shared.DTOs;

public class TicketFilterDto
{
    public Guid? PrioridadId { get; set; }
    public Guid? EstadoId { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? EmpleadoId { get; set; }
    public Guid? TecnicoId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}
