using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class PersonService(DataContext dbContext) : IPersonService
{
    public async Task<IReadOnlyCollection<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var people = await dbContext.People
            .Include(p => p.User)
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return people
            .Select(p => new PersonDto(
                p.Id,
                p.Name,
                p.Age,
                p.User != null,
                p.User?.Username,
                p.User?.Role.ToString()))
            .ToList();
    }

    public async Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.People
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return person is null
            ? null
            : new PersonDto(
                person.Id,
                person.Name,
                person.Age,
                person.User != null,
                person.User?.Username,
                person.User?.Role.ToString());
    }

    public async Task<PersonDto> CreateAsync(PersonInputDto input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length > 200)
        {
            throw new ArgumentException("Nome é obrigatório e deve ter no máximo 200 caracteres.", nameof(input.Name));
        }

        if (input.Age < 0)
        {
            throw new ArgumentException("Idade não pode ser negativa.", nameof(input.Age));
        }

        var entity = new Person
        {
            Name = input.Name.Trim(),
            Age = input.Age,
        };

        dbContext.People.Add(entity);

        if (input.CreateUser)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || input.Username.Length > 100)
            {
                throw new ArgumentException("Username é obrigatório e deve ter no máximo 100 caracteres.", nameof(input.Username));
            }

            if (string.IsNullOrWhiteSpace(input.Password) || input.Password.Length < 4)
            {
                throw new ArgumentException("Senha é obrigatória e deve ter pelo menos 4 caracteres.", nameof(input.Password));
            }

            var normalizedUsername = input.Username.Trim();

            var usernameExists = await dbContext.Users
                .AnyAsync(u => u.Username == normalizedUsername, cancellationToken);

            if (usernameExists)
            {
                throw new InvalidOperationException("Já existe um usuário com esse username.");
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(input.Password);

            var role = input.IsAdmin ? UserRole.Admin : UserRole.User;

            dbContext.Users.Add(new User
            {
                Username = normalizedUsername,
                PasswordHash = hash,
                Role = role,
                Person = entity,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new PersonDto(
            entity.Id,
            entity.Name,
            entity.Age,
            entity.User != null,
            entity.User?.Username,
            entity.User?.Role.ToString());
    }

    public async Task<PersonDto?> UpdateAsync(int id, PersonInputDto input, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.People
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (person is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length > 200)
        {
            throw new ArgumentException("Nome é obrigatório e deve ter no máximo 200 caracteres.", nameof(input.Name));
        }

        if (input.Age < 0)
        {
            throw new ArgumentException("Idade não pode ser negativa.", nameof(input.Age));
        }

        person.Name = input.Name.Trim();
        person.Age = input.Age;

        if (input.CreateUser)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || input.Username.Length > 100)
            {
                throw new ArgumentException("Username é obrigatório e deve ter no máximo 100 caracteres.", nameof(input.Username));
            }

            var normalizedUsername = input.Username.Trim();

            var usernameExists = await dbContext.Users
                .AnyAsync(u => u.Username == normalizedUsername && u.PersonId != person.Id, cancellationToken);

            if (usernameExists)
            {
                throw new InvalidOperationException("Já existe um usuário com esse username.");
            }

            var role = input.IsAdmin ? UserRole.Admin : UserRole.User;

            if (person.User is null)
            {
                // Cria usuário novo vinculado a esta pessoa.
                if (string.IsNullOrWhiteSpace(input.Password) || input.Password.Length < 4)
                {
                    throw new ArgumentException("Senha é obrigatória e deve ter pelo menos 4 caracteres.", nameof(input.Password));
                }

                var hash = BCrypt.Net.BCrypt.HashPassword(input.Password);

                dbContext.Users.Add(new User
                {
                    Username = normalizedUsername,
                    PasswordHash = hash,
                    Role = role,
                    Person = person,
                });
            }
            else
            {
                // Atualiza usuário existente.
                person.User.Username = normalizedUsername;
                person.User.Role = role;

                // Se uma nova senha foi informada, atualiza o hash.
                if (!string.IsNullOrWhiteSpace(input.Password))
                {
                    person.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new PersonDto(
            person.Id,
            person.Name,
            person.Age,
            person.User != null,
            person.User?.Username,
            person.User?.Role.ToString());
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.People.FindAsync([id], cancellationToken);
        if (person is null)
        {
            return false;
        }

        dbContext.People.Remove(person);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

