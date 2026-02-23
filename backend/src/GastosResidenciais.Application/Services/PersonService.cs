using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class PersonService(DataContext dbContext) : IPersonService
{
    public async Task<IReadOnlyCollection<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var people = await dbContext.People
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return people
            .Select(p => new PersonDto(p.Id, p.Name, p.Age))
            .ToList();
    }

    public async Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.People
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return person is null ? null : new PersonDto(person.Id, person.Name, person.Age);
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
        await dbContext.SaveChangesAsync(cancellationToken);

        return new PersonDto(entity.Id, entity.Name, entity.Age);
    }

    public async Task<PersonDto?> UpdateAsync(int id, PersonInputDto input, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.People.FindAsync([id], cancellationToken);
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return new PersonDto(person.Id, person.Name, person.Age);
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

