using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Application.Abstractions;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class CategoryService(IDataContext dbContext) : ICategoryService
{
    public async Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Description)
            .ToListAsync(cancellationToken);

        return categories
            .Select(c => new CategoryDto(c.Id, c.Description, c.Purpose, c.ColorHex))
            .ToList();
    }

    public async Task<CategoryDto> CreateAsync(CategoryInputDto input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Description) || input.Description.Length > 400)
        {
            throw new ArgumentException("Descrição é obrigatória e deve ter no máximo 400 caracteres.", nameof(input.Description));
        }

        if (!Enum.IsDefined(typeof(CategoryPurpose), input.Purpose))
        {
            throw new ArgumentException("Finalidade de categoria inválida.", nameof(input.Purpose));
        }

        var entity = new Category
        {
            Description = input.Description.Trim(),
            Purpose = input.Purpose,
            ColorHex = string.IsNullOrWhiteSpace(input.ColorHex)
                ? null
                : input.ColorHex.Trim(),
        };

        dbContext.Categories.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto(entity.Id, entity.Description, entity.Purpose, entity.ColorHex);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CategoryInputDto input, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input.Description) || input.Description.Length > 400)
        {
            throw new ArgumentException("Descrição é obrigatória e deve ter no máximo 400 caracteres.", nameof(input.Description));
        }

        if (!Enum.IsDefined(typeof(CategoryPurpose), input.Purpose))
        {
            throw new ArgumentException("Finalidade de categoria inválida.", nameof(input.Purpose));
        }

        entity.Description = input.Description.Trim();
        entity.Purpose = input.Purpose;
        entity.ColorHex = string.IsNullOrWhiteSpace(input.ColorHex)
            ? null
            : input.ColorHex.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto(entity.Id, entity.Description, entity.Purpose, entity.ColorHex);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.Categories.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

