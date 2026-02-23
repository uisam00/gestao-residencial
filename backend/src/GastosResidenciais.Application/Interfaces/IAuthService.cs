using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

/// <summary>
/// Serviço de autenticação: valida credenciais e garante usuário admin inicial.
/// </summary>
public interface IAuthService
{
    /// <summary>Valida usuário/senha e retorna os dados do usuário se válido.</summary>
    Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>Garante que existe um usuário admin (senha "admin") para uso inicial.</summary>
    Task EnsureAdminExistsAsync(CancellationToken cancellationToken = default);
}
