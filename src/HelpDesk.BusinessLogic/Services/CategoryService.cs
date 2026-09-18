using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public class CategoryService : BaseService, ICategoryService
{
    public CategoryService(
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

    public async Task<CategoryResponseDTO> CreateAsync(CategoryCreateDTO dto)
    {
        var existing = await _categoryRepository.GetByNameAsync(dto.Nombre);
        if (existing != null)
            throw new DuplicateException(ErrorMessages.TeamNameAlreadyExists);

        var category = new Category
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Activo = true
        };

        var created = await _categoryRepository.CreateAsync(category);

        return await MapToResponseDTO(created);
    }

    public async Task<CategoryResponseDTO?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return null;

        return await MapToResponseDTO(category);
    }

    public async Task<IEnumerable<CategoryListDTO>> GetAllAsync(bool soloActivos = true)
    {
        IEnumerable<Category> categories;
        if (soloActivos)
            categories = await _categoryRepository.GetActiveAsync();
        else
            categories = await _categoryRepository.GetAllAsync();

        var result = new List<CategoryListDTO>();
        foreach (var cat in categories)
        {
            var ticketsCount = (await _ticketRepository.GetFilteredAsync(categoriaId: cat.Id)).Count();
            result.Add(new CategoryListDTO(cat.Id, cat.Nombre, cat.Descripcion, cat.Activo, ticketsCount));
        }

        return result;
    }

    public async Task<CategoryResponseDTO> UpdateAsync(Guid id, CategoryUpdateDTO dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id, asNoTracking: false);
        if (category == null)
            throw new NotFoundException(ErrorMessages.CategoryNotFound);

        if (dto.Nombre != null)
        {
            var existing = await _categoryRepository.GetByNameAsync(dto.Nombre);
            if (existing != null && existing.Id != id)
                throw new DuplicateException(ErrorMessages.TeamNameAlreadyExists);
            category.Nombre = dto.Nombre;
        }

        if (dto.Descripcion != null)
            category.Descripcion = dto.Descripcion;

        if (dto.Activo.HasValue)
            category.Activo = dto.Activo.Value;

        await _categoryRepository.UpdateAsync(category);

        return await MapToResponseDTO(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id, asNoTracking: false);
        if (category == null)
            throw new NotFoundException(ErrorMessages.CategoryNotFound);

        var ticketsCount = (await _ticketRepository.GetFilteredAsync(categoriaId: id)).Count();
        if (ticketsCount > 0)
            throw new DependencyException(ErrorMessages.CategoryHasTickets);

        await _categoryRepository.DeleteAsync(id);
    }

    private async Task<CategoryResponseDTO> MapToResponseDTO(Category category)
    {
        var ticketsCount = (await _ticketRepository.GetFilteredAsync(categoriaId: category.Id)).Count();
        return new CategoryResponseDTO(category.Id, category.Nombre, category.Descripcion, category.Activo, ticketsCount);
    }
}