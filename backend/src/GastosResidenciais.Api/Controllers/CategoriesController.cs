using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // qualquer usuário autenticado pode consultar categorias
public class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await service.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // apenas admin pode criar categorias
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryInputDto input, CancellationToken cancellationToken)
    {
        try
        {
            var created = await service.CreateAsync(input, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto?>> Update(int id, [FromBody] CategoryInputDto input, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await service.UpdateAsync(id, input, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

