using GastosResidenciais.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Abstractions;

/// <summary>
/// Abstração do contexto de dados utilizada pela camada de aplicação.
/// Implementações concretas (por exemplo, EF Core) vivem na camada de infraestrutura.
/// </summary>
public interface IDataContext
{
    DbSet<Person> People { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

