using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Domain.Repositories;

public interface IPlantRepository
{
    Task<Plant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Plant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Plant>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Plant plant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Plant plant, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}