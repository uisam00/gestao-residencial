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
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] TransactionInputDto input, CancellationToken cancellationToken)
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

