using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GastosResidenciais.Infrastructure;

/// <summary>
/// Registra os serviços de infraestrutura.
/// </summary>
public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHostedService<MigrationHostedService>();
        
        return services;
    }
}