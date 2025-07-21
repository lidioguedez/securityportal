using Microsoft.EntityFrameworkCore;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Infrastructure.Data;

namespace SecurityPortal.Infrastructure.Repositories;

public class PlantRepository : IPlantRepository
{
    private readonly SecurityPortalDbContext _context;

    public PlantRepository(SecurityPortalDbContext context)
    {
        _context = context;
    }

    public async Task<Plant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .Include(p => p.Zones)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Plant>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .Include(p => p.Zones)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Plant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task AddAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        await _context.Plants.AddAsync(plant, cancellationToken);
    }

    public async Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _context.Plants.Update(plant);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _context.Plants.Remove(plant);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Plants.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Plants.AnyAsync(p => p.Code == code, cancellationToken);
    }
}