using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Retorna as transações visíveis para o usuário atual (aplicando regras de filtro por pessoa, se necessário).
    /// </summary>
    Task<IReadOnlyCollection<TransactionDto>> GetAllForUserAsync(string? role, int? personId, CancellationToken cancellationToken = default);
    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(TransactionInputDto input, CancellationToken cancellationToken = default);
    Task<TransactionDto?> UpdateAsync(int id, TransactionInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

