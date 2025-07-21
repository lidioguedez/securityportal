using Microsoft.EntityFrameworkCore;
using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Domain.ValueObjects;
using SecurityPortal.Infrastructure.Data;

namespace SecurityPortal.Infrastructure.Repositories;

public class SecurityIncidentRepository : ISecurityIncidentRepository
{
    private readonly SecurityPortalDbContext _context;

    public SecurityIncidentRepository(SecurityPortalDbContext context)
    {
        _context = context;
    }

    public async Task<SecurityIncident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<SecurityIncident?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Include(i => i.Comments)
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Where(i => i.PlantId == plantId)
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetByZoneIdAsync(Guid zoneId, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Where(i => i.ZoneId == zoneId)
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Where(i => i.Priority.Level == priority)
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetByAssigneeAsync(string assignee, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Where(i => i.AssignedTo == assignee)
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> GetRecentAsync(int days = 30, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await _context.SecurityIncidents
            .Where(i => i.OccurredAt >= cutoffDate)
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityIncident>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .Where(i => i.Title.Contains(searchTerm) || i.Description.Contains(searchTerm))
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SecurityIncident>> GetPaginatedAsync<T>(T specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default) 
        where T : ISpecification<SecurityIncident>
    {
        var query = ApplySpecification(_context.SecurityIncidents, specification);
        
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SecurityIncident incident, CancellationToken cancellationToken = default)
    {
        await _context.SecurityIncidents.AddAsync(incident, cancellationToken);
    }

    public async Task UpdateAsync(SecurityIncident incident, CancellationToken cancellationToken = default)
    {
        _context.SecurityIncidents.Update(incident);
        await Task.CompletedTask;
    }

    public void Update(SecurityIncident incident)
    {
        _context.SecurityIncidents.Update(incident);
    }

    public async Task DeleteAsync(SecurityIncident incident, CancellationToken cancellationToken = default)
    {
        _context.SecurityIncidents.Remove(incident);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .AnyAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .CountAsync(i => i.Status == status, cancellationToken);
    }

    public async Task<int> GetCountByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .CountAsync(i => i.Priority.Level == priority, cancellationToken);
    }

    private static IQueryable<SecurityIncident> ApplySpecification(IQueryable<SecurityIncident> query, ISpecification<SecurityIncident> spec)
    {
        // Apply criteria
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        // Apply includes
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);

        // Apply paging
        if (spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        return query;
    }
}