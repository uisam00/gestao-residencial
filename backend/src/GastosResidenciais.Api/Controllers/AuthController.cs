using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GastosResidenciais.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IJwtTokenGenerator tokenGenerator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized();
        }

        var token = tokenGenerator.GenerateToken(result.UserId, result.PersonId, result.Username, result.Role);

        return Ok(new
        {
            token,
            username = result.Username,
            personName = result.PersonName,
            role = result.Role,
            personId = result.PersonId
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserDto>> Me(CancellationToken cancellationToken)
    {
        var personIdClaim = User.FindFirst("person_id")?.Value;
        if (!int.TryParse(personIdClaim, out var personId))
        {
            return Unauthorized();
        }

        var current = await authService.GetCurrentUserAsync(personId, cancellationToken);
        if (current is null)
        {
            return NotFound();
        }

        return Ok(current);
    }
}

