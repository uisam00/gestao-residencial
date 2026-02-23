
using Microsoft.Extensions.DependencyInjection;

namespace GastosResidenciais.Application;


/// <summary>
/// Registra os serviços de aplicação.
/// </summary>
public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddScoped<ICategoryService, CategoryService>();
        //services.AddScoped<ITransactionService, TransactionService>();
        //services.AddScoped<IReportService, ReportService>();
        //services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}