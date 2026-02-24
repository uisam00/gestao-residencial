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
        var transactions = await service.GetAllAsync(cancellationToken);

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var personIdClaim = User.FindFirst("person_id")?.Value;

        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) && int.TryParse(personIdClaim, out var personId))
        {
            transactions = transactions.Where(t => t.PersonId == personId).ToArray();
        }

        return Ok(transactions);
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

