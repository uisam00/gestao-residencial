using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class ReportService(DataContext dbContext) : IReportService
{
    public async Task<PersonTotalsSummaryDto> GetTotalsByPersonAsync(CancellationToken cancellationToken = default)
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
            var income = person.Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = person.Transactions
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

    public async Task<CategoryTotalsSummaryDto> GetTotalsByCategoryAsync(CancellationToken cancellationToken = default)
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
            var income = category.Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = category.Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            totalIncome += income;
            totalExpense += expense;

            items.Add(new CategoryTotalsDto(
                category.Id,
                category.Description,
                category.Purpose,
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

