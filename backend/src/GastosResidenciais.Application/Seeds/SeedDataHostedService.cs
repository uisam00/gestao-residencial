using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace GastosResidenciais.Application.Seeds;

/// <summary>
/// Popula categorias iniciais, pessoas da família e transações de exemplo quando o banco está vazio.
/// Executado após o SeedAdminHostedService.
/// Só executa se RUN_SEED for true/1 (padrão: true).
/// </summary>
public class SeedDataHostedService(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        if (!SeedAdminHostedService.IsSeedEnabled(config))
            return;

        var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
        var personService = scope.ServiceProvider.GetRequiredService<IPersonService>();
        var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

        var categories = (await categoryService.GetAllAsync(cancellationToken)).ToList();
        if (categories.Count == 0)
        {
            await SeedCategoriesAsync(categoryService, cancellationToken);
            categories = (await categoryService.GetAllAsync(cancellationToken)).ToList();
        }

        var people = (await personService.GetAllAsync(cancellationToken)).ToList();
        if (people.Count <= 1)
        {
            await SeedPeopleAsync(personService, cancellationToken);
            people = (await personService.GetAllAsync(cancellationToken)).ToList();
        }

        var transactions = (await transactionService.GetAllAsync(cancellationToken)).ToList();
        if (transactions.Count == 0)
        {
            await SeedTransactionsAsync(transactionService, categories, people, cancellationToken);
        }
    }

    private static async Task SeedCategoriesAsync(ICategoryService categoryService, CancellationToken ct)
    {
        var toCreate = new[]
        {
            new CategoryInputDto("Alimentação", CategoryPurpose.Expense, "#dc2626"),
            new CategoryInputDto("Moradia", CategoryPurpose.Expense, "#7c3aed"),
            new CategoryInputDto("Renda extra", CategoryPurpose.Income, "#a3e635"),
            new CategoryInputDto("Salário", CategoryPurpose.Income, "#22c55e"),
            new CategoryInputDto("Saúde", CategoryPurpose.Expense, "#3b82f6"),
        };
        foreach (var dto in toCreate)
            await categoryService.CreateAsync(dto, ct);
    }

    private static async Task SeedPeopleAsync(IPersonService personService, CancellationToken ct)
    {
        var toCreate = new[]
        {
            new PersonInputDto("João Silva", 35, false, null, null, false),
            new PersonInputDto("Maria Silva", 32, false, null, null, false),
            new PersonInputDto("Pedro Silva", 10, false, null, null, false),
            new PersonInputDto("Ana Silva", 8, false, null, null, false),
        };
        foreach (var dto in toCreate)
            await personService.CreateAsync(dto, ct);
    }

    private static async Task SeedTransactionsAsync(
        ITransactionService transactionService,
        IReadOnlyCollection<CategoryDto> categories,
        IReadOnlyCollection<PersonDto> people,
        CancellationToken ct)
    {
        var cat = categories.ToDictionary(c => c.Description, c => c.Id);
        var fam = people.Where(p => !p.HasUser).Take(4).ToList();
        if (fam.Count == 0) return;

        var joao = fam.FirstOrDefault(p => p.Name.Contains("João")) ?? fam[0];
        var maria = fam.FirstOrDefault(p => p.Name.Contains("Maria")) ?? (fam.Count > 1 ? fam[1] : fam[0]);

        var toCreate = new[]
        {
            new TransactionInputDto("Salário mensal", 5500m, TransactionType.Income, cat["Salário"], joao.Id),
            new TransactionInputDto("Freela site", 1200m, TransactionType.Income, cat["Renda extra"], maria.Id),
            new TransactionInputDto("Supermercado", 850m, TransactionType.Expense, cat["Alimentação"], joao.Id),
            new TransactionInputDto("Aluguel", 1800m, TransactionType.Expense, cat["Moradia"], joao.Id),
            new TransactionInputDto("Plano de saúde", 420m, TransactionType.Expense, cat["Saúde"], maria.Id),
            new TransactionInputDto("Farmácia", 95m, TransactionType.Expense, cat["Saúde"], joao.Id),
            new TransactionInputDto("Feira", 120m, TransactionType.Expense, cat["Alimentação"], maria.Id),
        };

        foreach (var dto in toCreate)
            await transactionService.CreateAsync(dto, ct);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
