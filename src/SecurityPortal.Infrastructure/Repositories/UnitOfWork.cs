using SecurityPortal.Domain.Repositories;
using SecurityPortal.Infrastructure.Data;

namespace SecurityPortal.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SecurityPortalDbContext _context;
    private bool _disposed = false;

    private IPlantRepository? _plants;
    private ISecurityIncidentRepository? _securityIncidents;
    private ISecurityAlertRepository? _securityAlerts;

    public UnitOfWork(SecurityPortalDbContext context)
    {
        _context = context;
    }

    public IPlantRepository Plants
    {
        get => _plants ??= new PlantRepository(_context);
    }

    public ISecurityIncidentRepository SecurityIncidents
    {
        get => _securityIncidents ??= new SecurityIncidentRepository(_context);
    }

    public ISecurityAlertRepository SecurityAlerts
    {
        get => _securityAlerts ??= new SecurityAlertRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction == null)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.CurrentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Database.CurrentTransaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CurrentTransaction.DisposeAsync();
        }
        await _context.DisposeAsync();
        
        Dispose(false);
        GC.SuppressFinalize(this);
    }
}