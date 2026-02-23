using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Domain.Entities;

/// <summary>
/// Categoria usada para classificar uma transação como despesa ou receita.
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

