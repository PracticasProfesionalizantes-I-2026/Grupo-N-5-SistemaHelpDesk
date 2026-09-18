using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public class TeamService : BaseService, ITeamService
{
    public TeamService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IPriorityRepository priorityRepository,
        IStatusRepository statusRepository,
        ICommentRepository commentRepository,
        IStatusHistoryRepository statusHistoryRepository,
        ITeamRepository teamRepository)
        : base(ticketRepository, userRepository, categoryRepository, priorityRepository, statusRepository, commentRepository, statusHistoryRepository, teamRepository)
    {
    }

    public async Task<TeamResponseDTO> CreateAsync(TeamCreateDTO dto)
    {
        var existing = await _teamRepository.GetByNameAsync(dto.Nombre);
        if (existing != null)
            throw new DuplicateException(ErrorMessages.TeamNameAlreadyExists);

        if (dto.TecnicoIds == null || !dto.TecnicoIds.Any())
            throw new ValidationException("TecnicoIds", ErrorMessages.TeamMustHaveAtLeastOneTechnician);

        await ValidateCategoryExistsAsync(dto.CategoriaId);

        foreach (var tecnicoId in dto.TecnicoIds)
        {
            await ValidateTechnicianAsync(tecnicoId);
        }

        var team = new Team
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            CategoriaId = dto.CategoriaId,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        var created = await _teamRepository.CreateAsync(team);

        foreach (var tecnicoId in dto.TecnicoIds)
        {
            await _teamRepository.AddTecnicoAsync(created.Id, tecnicoId);
        }

        return await MapToResponseDTO(created);
    }

    public async Task<TeamResponseDTO?> GetByIdAsync(Guid id)
    {
        var team = await _teamRepository.GetWithTecnicosAsync(id);
        if (team == null)
            return null;

        return await MapToResponseDTO(team);
    }

    public async Task<PagedResultDTO<TeamListDTO>> GetFilteredAsync(TeamFilterDTO filter)
    {
        var teams = await _teamRepository.GetFilteredAsync(
            filter.CategoriaId,
            filter.Activo,
            filter.Search,
            filter.Page,
            filter.PageSize);

        var totalCount = await _teamRepository.GetFilteredCountAsync(
            filter.CategoriaId,
            filter.Activo,
            filter.Search);

        var itemDTOs = new List<TeamListDTO>();
        foreach (var team in teams)
        {
            var ticketsCount = (await _ticketRepository.GetFilteredAsync()).Count(t => t.TeamId == team.Id);
            itemDTOs.Add(new TeamListDTO(
                team.Id,
                team.Nombre,
                team.Descripcion,
                new CategoryResponseDTO(team.Categoria.Id, team.Categoria.Nombre, team.Categoria.Descripcion, team.Categoria.Activo, 0),
                team.Tecnicos.Count,
                team.FechaCreacion,
                team.Activo,
                ticketsCount
            ));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        return new PagedResultDTO<TeamListDTO>(itemDTOs, totalCount, filter.Page, filter.PageSize, totalPages);
    }

    public async Task<TeamResponseDTO> UpdateAsync(Guid id, TeamUpdateDTO dto)
    {
        var team = await _teamRepository.GetWithTecnicosAsync(id, asNoTracking: false);
        if (team == null)
            throw new NotFoundException(ErrorMessages.TeamNotFound);

        if (dto.Nombre != null)
        {
            var existing = await _teamRepository.GetByNameAsync(dto.Nombre);
            if (existing != null && existing.Id != id)
                throw new DuplicateException(ErrorMessages.TeamNameAlreadyExists);
            team.Nombre = dto.Nombre;
        }

        if (dto.Descripcion != null)
            team.Descripcion = dto.Descripcion;

        if (dto.CategoriaId.HasValue)
        {
            await ValidateCategoryExistsAsync(dto.CategoriaId.Value);
            team.CategoriaId = dto.CategoriaId.Value;
        }

        if (dto.Activo.HasValue)
            team.Activo = dto.Activo.Value;

        if (dto.TecnicoIds != null)
        {
            foreach (var tecnicoId in dto.TecnicoIds)
            {
                await ValidateTechnicianAsync(tecnicoId);
            }

            var currentTecnicoIds = team.Tecnicos.Select(t => t.Id).ToList();
            var toAdd = dto.TecnicoIds.Except(currentTecnicoIds);
            var toRemove = currentTecnicoIds.Except(dto.TecnicoIds);

            foreach (var tecnicoId in toAdd)
                await _teamRepository.AddTecnicoAsync(id, tecnicoId);

            foreach (var tecnicoId in toRemove)
                await _teamRepository.RemoveTecnicoAsync(id, tecnicoId);
        }

        await _teamRepository.UpdateAsync(team);

        return await MapToResponseDTO(team);
    }

    public async Task DeleteAsync(Guid id)
    {
        var team = await _teamRepository.GetByIdAsync(id, asNoTracking: false);
        if (team == null)
            throw new NotFoundException(ErrorMessages.TeamNotFound);

        var ticketsCount = (await _ticketRepository.GetFilteredAsync()).Count(t => t.TeamId == id);
        if (ticketsCount > 0)
            throw new DependencyException(ErrorMessages.TeamHasTickets);

        await _teamRepository.DeleteAsync(id);
    }

    public async Task<TeamResponseDTO> AddTecnicoAsync(Guid id, TeamAddTecnicoDTO dto)
    {
        var team = await _teamRepository.GetByIdAsync(id, asNoTracking: false);
        if (team == null)
            throw new NotFoundException(ErrorMessages.TeamNotFound);

        await ValidateTechnicianAsync(dto.TecnicoId);

        if (await _teamRepository.HasTecnicoAsync(id, dto.TecnicoId))
            throw new BusinessRuleException("El técnico ya pertenece a este equipo");

        await _teamRepository.AddTecnicoAsync(id, dto.TecnicoId);

        var updated = await _teamRepository.GetWithTecnicosAsync(id);
        return await MapToResponseDTO(updated!);
    }

    public async Task<TeamResponseDTO> RemoveTecnicoAsync(Guid id, TeamRemoveTecnicoDTO dto)
    {
        var team = await _teamRepository.GetByIdAsync(id, asNoTracking: false);
        if (team == null)
            throw new NotFoundException(ErrorMessages.TeamNotFound);

        if (!await _teamRepository.HasTecnicoAsync(id, dto.TecnicoId))
            throw new NotFoundException("El técnico no pertenece a este equipo");

        await _teamRepository.RemoveTecnicoAsync(id, dto.TecnicoId);

        var updated = await _teamRepository.GetWithTecnicosAsync(id);
        return await MapToResponseDTO(updated!);
    }

    private async Task<TeamResponseDTO> MapToResponseDTO(Team team)
    {
        var categoria = await _categoryRepository.GetByIdAsync(team.CategoriaId);
        var tecnicos = new List<UserSummaryDTO>();
        
        foreach (var t in team.Tecnicos)
        {
            tecnicos.Add(new UserSummaryDTO(t.Id, t.NombreCompleto, t.Email, t.Rol.ToString()));
        }

        var ticketsCount = (await _ticketRepository.GetFilteredAsync()).Count(t => t.TeamId == team.Id);

        return new TeamResponseDTO(
            team.Id,
            team.Nombre,
            team.Descripcion,
            new CategoryResponseDTO(categoria!.Id, categoria.Nombre, categoria.Descripcion, categoria.Activo, 0),
            tecnicos,
            team.FechaCreacion,
            team.Activo,
            ticketsCount
        );
    }
}