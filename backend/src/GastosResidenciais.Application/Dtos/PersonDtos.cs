namespace GastosResidenciais.Application.Dtos;

public record PersonDto(int Id, string Name, int Age);
public record PersonInputDto(string Name, int Age);

