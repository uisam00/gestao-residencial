namespace GastosResidenciais.Domain.Entities;

/// <summary>
/// Entidade que representa uma pessoa da residência que poderá ter transações financeiras associadas.
/// </summary>
public class Person
{
    /// <summary>
    /// Identificador único gerado automaticamente.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da pessoa (até 200 caracteres).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa em anos completos.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Usuário associado (caso esta pessoa tenha acesso ao sistema).
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Coleção de transações associadas à pessoa.
    /// Usado pelo EF Core para configurar o relacionamento e suportar deleção em cascata.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
