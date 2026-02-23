using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Domain.Entities;

/// <summary>
/// Transação financeira associada a uma pessoa e categoria.
/// </summary>
public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int PersonId { get; set; }
    public Person? Person { get; set; }
}

