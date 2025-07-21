using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.Entities;

public enum ZoneRiskLevel
{
    Safe = 1,
    LowRisk = 2,
    MediumRisk = 3,
    HighRisk = 4,
    Critical = 5
}

public class Zone : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ZoneRiskLevel RiskLevel { get; private set; }
    public Guid PlantId { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<SecurityIncident> _incidents = new();
    public IReadOnlyCollection<SecurityIncident> Incidents => _incidents.AsReadOnly();

    private Zone() { }

    public static Zone Create(string name, string description, ZoneRiskLevel riskLevel, Guid plantId, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Zone name cannot be empty", nameof(name));

        return new Zone
        {
            Name = name,
            Description = description,
            RiskLevel = riskLevel,
            PlantId = plantId,
            IsActive = true,
            CreatedBy = createdBy
        };
    }

    public void UpdateRiskLevel(ZoneRiskLevel newRiskLevel, string updatedBy)
    {
        RiskLevel = newRiskLevel;
        UpdateTimestamp(updatedBy);
    }

    public void UpdateDetails(string name, string description, string updatedBy)
    {
        Name = name;
        Description = description;
        UpdateTimestamp(updatedBy);
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        UpdateTimestamp(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        IsActive = true;
        UpdateTimestamp(updatedBy);
    }
}