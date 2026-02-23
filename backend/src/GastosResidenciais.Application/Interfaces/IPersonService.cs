using GastosResidenciais.Application.Dtos;

namespace GastosResidenciais.Application.Interfaces;

public interface IPersonService
{
    Task<IReadOnlyCollection<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PersonDto> CreateAsync(PersonInputDto input, CancellationToken cancellationToken = default);
    Task<PersonDto?> UpdateAsync(int id, PersonInputDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

