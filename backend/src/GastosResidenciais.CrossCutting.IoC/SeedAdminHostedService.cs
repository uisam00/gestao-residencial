using Microsoft.Extensions.Hosting;
using GastosResidenciais.Application.Interfaces;

namespace GastosResidenciais.CrossCutting.IoC;

/// <summary>
/// Garante que o usuário admin inicial existe após as migrations.
/// Recebe IAuthService via DI; registrado no CrossCutting para não criar referência Infrastructure → Application.
/// </summary>
public class SeedAdminHostedService(IAuthService authService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await authService.EnsureAdminExistsAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
