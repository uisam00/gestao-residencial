using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GastosResidenciais.Application;
using GastosResidenciais.Infrastructure;

namespace GastosResidenciais.CrossCutting.IoC;

/// <summary>
/// Ponto central de configuração de injeção de dependência do backend.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra DbContext, serviços de aplicação e demais dependências.
    /// </summary>
    public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        // Infrastructure primeiro para MigrationHostedService rodar antes do SeedAdmin.
        services.AddInfrastructure(configuration);
        services.AddApplication();

        return services;
    }
}
