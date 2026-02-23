namespace GastosResidenciais.Domain.Enums;

/// <summary>
/// Define a finalidade principal de uma categoria.
/// Essa enum é usada para validar se uma categoria pode ser utilizada em determinado tipo de transação.
/// </summary>
public enum CategoryPurpose
{
    Expense = 1,
    Income = 2,
    Both = 3
}

