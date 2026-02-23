using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

public interface IReportService
{
    Task<PersonTotalsSummaryDto> GetTotalsByPersonAsync(CancellationToken cancellationToken = default);
    Task<CategoryTotalsSummaryDto> GetTotalsByCategoryAsync(CancellationToken cancellationToken = default);
}

