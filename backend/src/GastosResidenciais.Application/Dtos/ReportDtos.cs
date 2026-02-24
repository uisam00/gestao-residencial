using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.Dtos;

public record PersonTotalsDto(
    int Id,
    string Name,
    int Age,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance);

public record CategoryTotalsDto(
    int Id,
    string Description,
    CategoryPurpose Purpose,
    string? ColorHex,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance);

public record PersonTotalsSummaryDto(
    IReadOnlyCollection<PersonTotalsDto> Items,
    decimal GrandTotalIncome,
    decimal GrandTotalExpense,
    decimal GrandBalance);

public record CategoryTotalsSummaryDto(
    IReadOnlyCollection<CategoryTotalsDto> Items,
    decimal GrandTotalIncome,
    decimal GrandTotalExpense,
    decimal GrandBalance);

