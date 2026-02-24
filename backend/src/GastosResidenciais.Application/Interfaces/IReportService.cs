using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.Interfaces;

public interface IReportService
{
    Task<PersonTotalsSummaryDto> GetTotalsByPersonAsync(int? personId, int? categoryId, TransactionType? type, CancellationToken cancellationToken = default);
    Task<CategoryTotalsSummaryDto> GetTotalsByCategoryAsync(int? personId, int? categoryId, TransactionType? type, CancellationToken cancellationToken = default);
}

