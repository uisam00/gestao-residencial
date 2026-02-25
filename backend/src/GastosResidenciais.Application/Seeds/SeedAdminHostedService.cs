using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GastosResidenciais.Application.Interfaces;

namespace GastosResidenciais.Application.Seeds;

/// <summary>
/// Garante que o usuário admin inicial existe após as migrations.
/// Recebe IAuthService via DI; registrado no CrossCutting para não criar referência Infrastructure → Application.
/// Só executa se RUN_SEED for true/1 (padrão: true).
/// </summary>
public class SeedAdminHostedService(IAuthService authService, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!IsSeedEnabled(configuration))
        {
            return;
        }

        await authService.EnsureAdminExistsAsync(cancellationToken);
    }

    internal static bool IsSeedEnabled(IConfiguration config)
    {
        var value = config["RUN_SEED"]?.Trim();

        return string.IsNullOrEmpty(value)
               || value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value == "1";
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
