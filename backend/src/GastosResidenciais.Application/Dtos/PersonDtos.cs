namespace GastosResidenciais.Application.Dtos;

/// <summary>
/// Dados de uma pessoa, incluindo informações resumidas do usuário vinculado (quando houver).
/// </summary>
public record PersonDto(
    int Id,
    string Name,
    int Age,
    bool HasUser,
    string? Username,
    string? Role);

/// <summary>
/// Dados de entrada para criação/edição de pessoa.
/// Opcionalmente pode criar/atualizar também um usuário vinculado a essa pessoa.
/// </summary>
public record PersonInputDto(
    string Name,
    int Age,
    bool CreateUser,
    string? Username,
    string? Password,
    bool IsAdmin);

