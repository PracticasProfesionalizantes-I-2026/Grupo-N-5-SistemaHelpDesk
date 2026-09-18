using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDTO>> Create([FromBody] CategoryCreateDTO dto)
    {
        try
        {
            var result = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryListDTO>>> GetAll([FromQuery] bool soloActivos = true)
    {
        try
        {
            var result = await _categoryService.GetAllAsync(soloActivos);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDTO>> GetById(Guid id)
    {
        try
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { error = "Categoría no encontrada" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponseDTO>> Update(Guid id, [FromBody] CategoryUpdateDTO dto)
    {
        try
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    private ActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            NotFoundException => NotFound(new { error = ex.Message }),
            ValidationException => BadRequest(new { error = ex.Message }),
            BusinessRuleException => Conflict(new { error = ex.Message }),
            DuplicateException => Conflict(new { error = ex.Message }),
            DependencyException => Conflict(new { error = ex.Message }),
            _ => StatusCode(500, new { error = "Error interno del servidor" })
        };
    }
}