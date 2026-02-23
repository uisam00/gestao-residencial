using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.Dtos;

public record CategoryDto(int Id, string Description, CategoryPurpose Purpose);
public record CategoryInputDto(string Description, CategoryPurpose Purpose);

