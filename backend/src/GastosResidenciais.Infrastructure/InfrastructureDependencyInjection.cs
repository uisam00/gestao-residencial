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

        return services;
    }
}