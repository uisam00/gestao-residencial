using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
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
            .Include(u => u.Person)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username.Trim(), cancellationToken);

        if (user is null || user.Person is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new LoginResult(
            user.Id,
            user.PersonId,
            user.Username,
            user.Role.ToString(),
            user.Person.Name);
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(int personId, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.People
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

        if (person is null || person.User is null)
        {
            return null;
        }

        return new CurrentUserDto(
            person.Id,
            person.Name,
            person.Age,
            person.User.Username,
            person.User.Role.ToString());
    }

    public async Task EnsureAdminExistsAsync(CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.Username == DefaultAdminUsername, cancellationToken);

        if (exists)
            return;

        var adminPerson = new Person
        {
            Name = "Administrador",
            Age = 0
        };

        var hash = BCrypt.Net.BCrypt.HashPassword(DefaultAdminPassword);
        dbContext.Users.Add(new User
        {
            Username = DefaultAdminUsername,
            PasswordHash = hash,
            Role = UserRole.Admin,
            Person = adminPerson,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
