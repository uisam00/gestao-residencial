using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(TransactionInputDto input, CancellationToken cancellationToken = default);
    Task<TransactionDto?> UpdateAsync(int id, TransactionInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

