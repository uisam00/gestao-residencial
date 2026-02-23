using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GastosResidenciais.Application.Interfaces;

namespace GastosResidenciais.Application.Seeds;

/// <summary>
/// Garante que o usuário admin inicial existe após as migrations.
/// Resolve IAuthService em um scope (scoped) a partir do container (singleton).
/// </summary>
public class SeedAdminHostedService(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        await authService.EnsureAdminExistsAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
