using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Domain.Entities;

/// <summary>
/// Usuário do sistema para autenticação (login na aplicação).
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Perfil de acesso do usuário (Admin ou User).
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// Chave estrangeira para a pessoa associada a este usuário.
    /// Todo usuário deve estar vinculado a uma pessoa.
    /// </summary>
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
}
