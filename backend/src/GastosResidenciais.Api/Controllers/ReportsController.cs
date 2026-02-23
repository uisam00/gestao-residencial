using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController(IReportService service) : ControllerBase
{
    [HttpGet("by-person")]
    public async Task<ActionResult<PersonTotalsSummaryDto>> GetByPerson(CancellationToken cancellationToken)
    {
        var summary = await service.GetTotalsByPersonAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<CategoryTotalsSummaryDto>> GetByCategory(CancellationToken cancellationToken)
    {
        var summary = await service.GetTotalsByCategoryAsync(cancellationToken);
        return Ok(summary);
    }
}

