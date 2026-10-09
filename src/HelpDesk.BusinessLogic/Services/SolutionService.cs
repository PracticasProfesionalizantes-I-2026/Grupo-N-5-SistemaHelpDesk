using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.Constants;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;

namespace HelpDesk.BusinessLogic.Services;

public class SolutionService : ISolutionService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISolutionRepository _solutionRepository;
    private readonly IStatusHistoryRepository _statusHistoryRepository;
    private readonly IStatusRepository _statusRepository;

    public SolutionService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        ISolutionRepository solutionRepository,
        IStatusHistoryRepository statusHistoryRepository,
        IStatusRepository statusRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _solutionRepository = solutionRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _statusRepository = statusRepository;
    }

    public async Task<SolutionFormDTO> GetFormAsync(
        Guid ticketId,
        Guid tecnicoId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);

        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        await ValidateTechnicianAccessAsync(ticket, tecnicoId);

        return new SolutionFormDTO(
            ticketId,
            BusinessRules.AllowedSolutionFileExtensions,
            BusinessRules.MaxSolutionFileSizeBytes,
            "10 MB");
    }

    public async Task<SolutionResponseDTO> CreateAsync(
        Guid ticketId,
        SolutionCreateDTO dto,
        Guid tecnicoId,
        string usuarioRol)
    {
        if (usuarioRol != UserRole.Tecnico.ToString())
            throw new UnauthorizedActionException(
                ErrorMessages.OnlyTechnicianCanAttachSolution);

        var tecnico = await _userRepository.GetByIdAsync(tecnicoId);

        if (tecnico == null)
            throw new NotFoundException(ErrorMessages.UserNotFound);

        if (tecnico.Rol != UserRole.Tecnico || !tecnico.Activo)
            throw new UnauthorizedActionException(
                ErrorMessages.OnlyTechnicianCanAttachSolution);

        var ticket = await _ticketRepository.GetByIdAsync(
            ticketId,
            asNoTracking: false);

        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        await ValidateTechnicianAccessAsync(ticket, tecnicoId);

        var currentStatus =
            await _statusRepository.GetByIdAsync(ticket.EstadoId);

        if (currentStatus?.EsFinal == true)
            throw new BusinessRuleException(
                ErrorMessages.CannotModifyClosedTicket);

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            throw new ValidationException(
                ErrorMessages.SolutionDescriptionRequired);

        if (dto.Descripcion.Length >
            BusinessRules.MaxSolutionDescriptionLength)
        {
            throw new ValidationException(
                "La descripción de la solución no puede superar los 5000 caracteres");
        }

        ValidateFile(dto);

        var solution = new Solution
        {
            TicketId = ticketId,
            TecnicoId = tecnicoId,
            Descripcion = dto.Descripcion.Trim(),
            NombreArchivo = dto.NombreArchivo,
            TipoContenido = dto.TipoContenido,
            TamanioArchivo = dto.TamanioArchivo,
            ContenidoArchivo = dto.ContenidoArchivo,
            FechaCreacion = DateTime.UtcNow
        };

        var created =
            await _solutionRepository.CreateAsync(solution);

        // Registramos la acción en el historial.
        // El estado no cambia, por eso EstadoAnteriorId y
        // EstadoNuevoId tienen el mismo valor.
        await _statusHistoryRepository.CreateAsync(
            new StatusHistory
            {
                TicketId = ticket.Id,
                EstadoAnteriorId = ticket.EstadoId,
                EstadoNuevoId = ticket.EstadoId,
                UsuarioId = tecnicoId,
                FechaCambio = DateTime.UtcNow,
                Observacion = "Solución técnica adjuntada"
            });

        ticket.FechaActualizacion = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);

        return MapToResponseDTO(created);
    }

    private async Task ValidateTechnicianAccessAsync(
        Ticket ticket,
        Guid tecnicoId)
    {
        if (ticket.TecnicoId != tecnicoId)
        {
            throw new UnauthorizedActionException(
                ErrorMessages.UnauthorizedAccess);
        }

        var tecnico =
            await _userRepository.GetByIdAsync(tecnicoId);

        if (tecnico == null)
            throw new NotFoundException(
                ErrorMessages.UserNotFound);

        if (tecnico.Rol != UserRole.Tecnico ||
            !tecnico.Activo)
        {
            throw new UnauthorizedActionException(
                ErrorMessages.OnlyTechnicianCanAttachSolution);
        }
    }

    private static void ValidateFile(
        SolutionCreateDTO dto)
    {
        // No hay archivo: es válido si solamente se registra
        // la descripción de la solución.
        if (dto.ContenidoArchivo == null ||
            dto.ContenidoArchivo.Length == 0)
        {
            if (!string.IsNullOrWhiteSpace(dto.NombreArchivo) ||
                dto.TamanioArchivo.HasValue)
            {
                throw new ValidationException(
                    ErrorMessages.SolutionFileTypeNotAllowed);
            }

            return;
        }

        if (dto.ContenidoArchivo.Length >
            BusinessRules.MaxSolutionFileSizeBytes)
        {
            throw new ValidationException(
                ErrorMessages.SolutionFileTooLarge);
        }

        if (string.IsNullOrWhiteSpace(dto.NombreArchivo))
        {
            throw new ValidationException(
                "El archivo adjunto debe tener un nombre");
        }

        var extension =
            Path.GetExtension(dto.NombreArchivo)
                .ToLowerInvariant();

        if (!BusinessRules.AllowedSolutionFileExtensions
            .Contains(extension))
        {
            throw new ValidationException(
                ErrorMessages.SolutionFileTypeNotAllowed);
        }
    }

    private static SolutionResponseDTO MapToResponseDTO(
        Solution solution)
    {
        return new SolutionResponseDTO(
            solution.Id,
            solution.TicketId,
            solution.TecnicoId,
            solution.Descripcion,
            solution.NombreArchivo,
            solution.TipoContenido,
            solution.TamanioArchivo,
            solution.FechaCreacion);
    }
}