using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class ReportService(DataContext dbContext) : IReportService
{
    public async Task<PersonTotalsSummaryDto> GetTotalsByPersonAsync(int? personId, int? categoryId, TransactionType? type, CancellationToken cancellationToken = default)
    {
        var peopleWithTransactions = await dbContext.People
            .Include(p => p.Transactions)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = new List<PersonTotalsDto>();

        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var person in peopleWithTransactions)
        {
            if (personId.HasValue && person.Id != personId.Value)
            {
                continue;
            }

            var transactions = person.Transactions.AsEnumerable();

            if (categoryId.HasValue)
            {
                transactions = transactions.Where(t => t.CategoryId == categoryId.Value);
            }

            if (type.HasValue)
            {
                transactions = transactions.Where(t => t.Type == type.Value);
            }

            var income = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            totalIncome += income;
            totalExpense += expense;

            items.Add(new PersonTotalsDto(
                person.Id,
                person.Name,
                person.Age,
                income,
                expense,
                income - expense));
        }

        return new PersonTotalsSummaryDto(
            items,
            totalIncome,
            totalExpense,
            totalIncome - totalExpense);
    }

    public async Task<CategoryTotalsSummaryDto> GetTotalsByCategoryAsync(int? personId, int? categoryId, TransactionType? type, CancellationToken cancellationToken = default)
    {
        var categoriesWithTransactions = await dbContext.Categories
            .Include(c => c.Transactions)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = new List<CategoryTotalsDto>();

        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var category in categoriesWithTransactions)
        {
            if (categoryId.HasValue && category.Id != categoryId.Value)
            {
                continue;
            }

            var transactions = category.Transactions.AsEnumerable();

            if (personId.HasValue)
            {
                transactions = transactions.Where(t => t.PersonId == personId.Value);
            }

            if (type.HasValue)
            {
                transactions = transactions.Where(t => t.Type == type.Value);
            }

            var income = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            totalIncome += income;
            totalExpense += expense;

            items.Add(new CategoryTotalsDto(
                category.Id,
                category.Description,
                category.Purpose,
                category.ColorHex,
                income,
                expense,
                income - expense));
        }

        return new CategoryTotalsSummaryDto(
            items,
            totalIncome,
            totalExpense,
            totalIncome - totalExpense);
    }
}

