using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(ITransactionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TransactionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var personIdClaim = User.FindFirst("person_id")?.Value;

        int? personId = int.TryParse(personIdClaim, out var parsedPersonId) ? parsedPersonId : null;
        var transactions = await service.GetAllForUserAsync(role, personId, cancellationToken);

        return Ok(transactions);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransactionDto>> Update(int id, [FromBody] TransactionInputDto input, CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var personIdClaim = User.FindFirst("person_id")?.Value;

        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(personIdClaim, out var personId) &&
            existing.PersonId != personId)
        {
            return Forbid();
        }

        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(personIdClaim, out var currentPersonId))
        {
            input = new TransactionInputDto(
                input.Description,
                input.Amount,
                input.Type,
                input.CategoryId,
                currentPersonId);
        }

        try
        {
            var updated = await service.UpdateAsync(id, input, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var personIdClaim = User.FindFirst("person_id")?.Value;

        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(personIdClaim, out var personId) &&
            existing.PersonId != personId)
        {
            return Forbid();
        }

        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] TransactionInputDto input, CancellationToken cancellationToken)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var personIdClaim = User.FindFirst("person_id")?.Value;

            if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) && int.TryParse(personIdClaim, out var personId))
            {
                // Usuário normal só pode lançar transações para ele mesmo.
                input = new TransactionInputDto(
                    input.Description,
                    input.Amount,
                    input.Type,
                    input.CategoryId,
                    personId);
            }

            var created = await service.CreateAsync(input, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

