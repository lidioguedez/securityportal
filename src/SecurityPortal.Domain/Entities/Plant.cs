using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.Entities;

public class Plant : BaseAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public Address Address { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    
    private readonly List<Zone> _zones = new();
    public IReadOnlyCollection<Zone> Zones => _zones.AsReadOnly();

    private Plant() { }

    public static Plant Create(string name, string code, Address address, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plant name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Plant code cannot be empty", nameof(code));

        var plant = new Plant
        {
            Name = name,
            Code = code,
            Address = address,
            IsActive = true,
            ActivatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        return plant;
    }

    public void AddZone(Zone zone)
    {
        if (_zones.Any(z => z.Name == zone.Name))
            throw new InvalidOperationException($"Zone with name {zone.Name} already exists");

        _zones.Add(zone);
        UpdateTimestamp(string.Empty);
    }

    public void RemoveZone(Zone zone)
    {
        _zones.Remove(zone);
        UpdateTimestamp(string.Empty);
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        UpdateTimestamp(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        IsActive = true;
        ActivatedAt = DateTime.UtcNow;
        UpdateTimestamp(updatedBy);
    }

    public void UpdateDetails(string name, Address address, string updatedBy)
    {
        Name = name;
        Address = address;
        UpdateTimestamp(updatedBy);
    }
}