using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PeopleController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PersonDto>>> GetAll(CancellationToken cancellationToken)
    {
        var people = await service.GetAllAsync(cancellationToken);
        return Ok(people);
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create([FromBody] PersonInputDto input, CancellationToken cancellationToken)
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
    public async Task<ActionResult<PersonDto?>> Update(int id, [FromBody] PersonInputDto input, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

