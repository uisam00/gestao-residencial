
using Microsoft.Extensions.DependencyInjection;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Application.Services;
using GastosResidenciais.Application.Seeds;

namespace GastosResidenciais.Application;


/// <summary>
/// Registra os serviços de aplicação.
/// </summary>
public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddHostedService<SeedAdminHostedService>();
        services.AddHostedService<SeedDataHostedService>();

        return services;
    }
}