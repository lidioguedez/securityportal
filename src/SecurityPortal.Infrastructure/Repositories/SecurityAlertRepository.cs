using Microsoft.EntityFrameworkCore;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Domain.ValueObjects;
using SecurityPortal.Infrastructure.Data;

namespace SecurityPortal.Infrastructure.Repositories;

public class SecurityAlertRepository : ISecurityAlertRepository
{
    private readonly SecurityPortalDbContext _context;

    public SecurityAlertRepository(SecurityPortalDbContext context)
    {
        _context = context;
    }

    public async Task<SecurityAlert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.PlantId == plantId)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetByZoneIdAsync(Guid zoneId, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.ZoneId == zoneId)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetByStatusAsync(AlertStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.Priority.Level == priority)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetByTypeAsync(AlertType type, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.Type == type)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.Status == AlertStatus.Active)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetUnacknowledgedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.Status == AlertStatus.Active && a.AcknowledgedAt == null)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .Where(a => a.Status == AlertStatus.Active)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SecurityAlert>> GetRecentAsync(int hours = 24, CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTime.UtcNow.AddHours(-hours);
        return await _context.SecurityAlerts
            .Where(a => a.TriggeredAt >= cutoffTime)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SecurityAlert alert, CancellationToken cancellationToken = default)
    {
        await _context.SecurityAlerts.AddAsync(alert, cancellationToken);
    }

    public async Task UpdateAsync(SecurityAlert alert, CancellationToken cancellationToken = default)
    {
        _context.SecurityAlerts.Update(alert);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(SecurityAlert alert, CancellationToken cancellationToken = default)
    {
        _context.SecurityAlerts.Remove(alert);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(AlertStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .CountAsync(a => a.Status == status, cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .CountAsync(a => a.Status == AlertStatus.Active, cancellationToken);
    }

    public async Task<int> GetCountByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityAlerts
            .CountAsync(a => a.Priority.Level == priority, cancellationToken);
    }
}