namespace GastosResidenciais.Application.Dtos;

/// <summary>Payload enviado no login.</summary>
public record LoginRequest(string Username, string Password);

/// <summary>Dados retornados após login bem-sucedido (token é gerado na API).</summary>
public record LoginResult(int UserId, int PersonId, string Username, string Role, string PersonName);

/// <summary>Dados da pessoa/usuário autenticado para o endpoint /auth/me.</summary>
public record CurrentUserDto(int PersonId, string PersonName, int Age, string Username, string Role);
