using GastosResidenciais.Application.Dtos;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Application.Services;

public class CategoryService(DataContext dbContext) : ICategoryService
{
    public async Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Description)
            .ToListAsync(cancellationToken);

        return categories
            .Select(c => new CategoryDto(c.Id, c.Description, c.Purpose))
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
        };

        dbContext.Categories.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto(entity.Id, entity.Description, entity.Purpose);
    }
}

