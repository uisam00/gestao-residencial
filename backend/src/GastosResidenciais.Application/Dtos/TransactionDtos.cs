using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.Dtos;

public record TransactionDto(
    int Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    int CategoryId,
    int PersonId,
    string PersonName,
    string CategoryDescription,
    string? CategoryColorHex);

public record TransactionInputDto(
    string Description,
    decimal Amount,
    TransactionType Type,
    int CategoryId,
    int PersonId);

