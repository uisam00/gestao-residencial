using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Application.Abstractions;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class TransactionService(IDataContext dbContext) : ITransactionService
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
                t.Category?.Description ?? string.Empty,
                t.Category?.ColorHex))
            .ToList();
    }

    public async Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var t = await dbContext.Transactions
            .Include(tx => tx.Person)
            .Include(tx => tx.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(tx => tx.Id == id, cancellationToken);

        return t is null
            ? null
            : new TransactionDto(
                t.Id,
                t.Description,
                t.Amount,
                t.Type,
                t.CategoryId,
                t.PersonId,
                t.Person?.Name ?? string.Empty,
                t.Category?.Description ?? string.Empty,
                t.Category?.ColorHex);
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

        // Já temos person e category carregados acima, então usamos esses dados para o DTO.
        return new TransactionDto(
            entity.Id,
            entity.Description,
            entity.Amount,
            entity.Type,
            entity.CategoryId,
            entity.PersonId,
            person.Name,
            category.Description,
            category.ColorHex);
    }

    public async Task<TransactionDto?> UpdateAsync(int id, TransactionInputDto input, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Transactions.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return null;
        }

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

        entity.Description = input.Description.Trim();
        entity.Amount = input.Amount;
        entity.Type = input.Type;
        entity.CategoryId = input.CategoryId;
        entity.PersonId = input.PersonId;

        await dbContext.SaveChangesAsync(cancellationToken);

        // Também já temos person e category carregados neste método.
        return new TransactionDto(
            entity.Id,
            entity.Description,
            entity.Amount,
            entity.Type,
            entity.CategoryId,
            entity.PersonId,
            person.Name,
            category.Description,
            category.ColorHex);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Transactions.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.Transactions.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

