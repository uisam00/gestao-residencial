using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(TransactionInputDto input, CancellationToken cancellationToken = default);
}

