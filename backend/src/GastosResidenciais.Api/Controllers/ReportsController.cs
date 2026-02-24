using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController(IReportService service) : ControllerBase
{
    [HttpGet("by-person")]
    public async Task<ActionResult<PersonTotalsSummaryDto>> GetByPerson(
        [FromQuery] int? personId,
        [FromQuery] int? categoryId,
        [FromQuery] TransactionType? type,
        CancellationToken cancellationToken)
    {
        var summary = await service.GetTotalsByPersonAsync(personId, categoryId, type, cancellationToken);
        return Ok(summary);
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<CategoryTotalsSummaryDto>> GetByCategory(
        [FromQuery] int? personId,
        [FromQuery] int? categoryId,
        [FromQuery] TransactionType? type,
        CancellationToken cancellationToken)
    {
        var summary = await service.GetTotalsByCategoryAsync(personId, categoryId, type, cancellationToken);
        return Ok(summary);
    }
}

