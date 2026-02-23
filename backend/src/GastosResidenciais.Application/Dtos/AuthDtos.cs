namespace GastosResidenciais.Application.Dtos;

/// <summary>Payload enviado no login.</summary>
public record LoginRequest(string Username, string Password);

/// <summary>Dados retornados após login bem-sucedido (token é gerado na API).</summary>
public record LoginResult(int UserId, string Username);
