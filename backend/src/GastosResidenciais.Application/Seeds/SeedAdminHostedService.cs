using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GastosResidenciais.Application.Interfaces;

namespace GastosResidenciais.Application.Seeds;

/// <summary>
/// Garante que o usuário admin inicial existe após as migrations.
/// Resolve IAuthService em um scope (scoped) a partir do container (singleton).
/// Só executa se RUN_SEED for true/1 (padrão: true).
/// </summary>
public class SeedAdminHostedService(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        if (!IsSeedEnabled(config))
            return;

        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        await authService.EnsureAdminExistsAsync(cancellationToken);
    }

    internal static bool IsSeedEnabled(IConfiguration config)
    {
        var value = config["RUN_SEED"]?.Trim();
        return string.IsNullOrEmpty(value) || value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1";
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
