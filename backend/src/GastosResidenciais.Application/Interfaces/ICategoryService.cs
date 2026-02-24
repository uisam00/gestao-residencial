using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateAsync(CategoryInputDto input, CancellationToken cancellationToken = default);
    Task<CategoryDto?> UpdateAsync(int id, CategoryInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

