using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class TransactionService(DataContext dbContext) : ITransactionService
{
    public async Task<IReadOnlyCollection<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await dbContext.Transactions
            .AsNoTracking()
            .Include(t => t.Person)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

        return transactions
            .Select(t => new TransactionDto(
                t.Id,
                t.Description,
                t.Amount,
                t.Type,
                t.CategoryId,
                t.PersonId,
                t.Person?.Name ?? string.Empty,
                t.Category?.Description ?? string.Empty))
            .ToList();
    }

    public async Task<TransactionDto> CreateAsync(TransactionInputDto input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Description) || input.Description.Length > 400)
        {
            throw new ArgumentException("Descrição é obrigatória e deve ter no máximo 400 caracteres.", nameof(input.Description));
        }

        if (input.Amount <= 0)
        {
            throw new ArgumentException("Valor deve ser positivo.", nameof(input.Amount));
        }

        if (!Enum.IsDefined(typeof(TransactionType), input.Type))
        {
            throw new ArgumentException("Tipo de transação inválido.", nameof(input.Type));
        }

        var person = await dbContext.People.FindAsync([input.PersonId], cancellationToken);
        if (person is null)
        {
            throw new InvalidOperationException("Pessoa informada não existe.");
        }

        var category = await dbContext.Categories.FindAsync([input.CategoryId], cancellationToken);
        if (category is null)
        {
            throw new InvalidOperationException("Categoria informada não existe.");
        }

        if (person.Age < 18 && input.Type != TransactionType.Expense)
        {
            throw new InvalidOperationException("Para menores de 18 anos somente transações de despesa são permitidas.");
        }

        var isExpense = input.Type == TransactionType.Expense;
        var isIncome = input.Type == TransactionType.Income;

        var allowsExpense = category.Purpose is CategoryPurpose.Expense or CategoryPurpose.Both;
        var allowsIncome = category.Purpose is CategoryPurpose.Income or CategoryPurpose.Both;

        if (isExpense && !allowsExpense)
        {
            throw new InvalidOperationException("Categoria selecionada não aceita transações de despesa.");
        }

        if (isIncome && !allowsIncome)
        {
            throw new InvalidOperationException("Categoria selecionada não aceita transações de receita.");
        }

        var entity = new Transaction
        {
            Description = input.Description.Trim(),
            Amount = input.Amount,
            Type = input.Type,
            CategoryId = input.CategoryId,
            PersonId = input.PersonId,
        };

        dbContext.Transactions.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(entity).Reference(t => t.Person).LoadAsync(cancellationToken);
        await dbContext.Entry(entity).Reference(t => t.Category).LoadAsync(cancellationToken);

        return new TransactionDto(
            entity.Id,
            entity.Description,
            entity.Amount,
            entity.Type,
            entity.CategoryId,
            entity.PersonId,
            entity.Person?.Name ?? string.Empty,
            entity.Category?.Description ?? string.Empty);
    }
}

