namespace GastosResidenciais.Domain.Entities;

/// <summary>
/// Usuário do sistema para autenticação (login na aplicação).
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
