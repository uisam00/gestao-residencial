using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class AuthService(DataContext dbContext) : IAuthService
{
    private const string DefaultAdminUsername = "admin";
    private const string DefaultAdminPassword = "admin";

    public async Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return null;

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username.Trim(), cancellationToken);

        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new LoginResult(user.Id, user.Username);
    }

    public async Task EnsureAdminExistsAsync(CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.Username == DefaultAdminUsername, cancellationToken);

        if (exists)
            return;

        var hash = BCrypt.Net.BCrypt.HashPassword(DefaultAdminPassword);
        dbContext.Users.Add(new User
        {
            Username = DefaultAdminUsername,
            PasswordHash = hash,
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
