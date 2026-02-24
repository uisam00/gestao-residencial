using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GastosResidenciais.Infrastructure.Context;
using GastosResidenciais.Application.Abstractions;
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

        // Expõe o DataContext também pela abstração IDataContext usada na camada de aplicação.
        services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());

        services.AddHostedService<MigrationHostedService>();
        
        return services;
    }
}