using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Domain.Specifications;

public class IncidentFilterSpecification : BaseSpecification<SecurityIncident>
{
    public IncidentFilterSpecification(
        IncidentStatus? status = null,
        Guid? plantId = null,
        Guid? zoneId = null,
        string? assignedTo = null,
        DateTime? fromDate = null,
        DateTime? toDate = null) 
        : base(x => 
            (!status.HasValue || x.Status == status.Value) &&
            (!plantId.HasValue || x.PlantId == plantId.Value) &&
            (!zoneId.HasValue || x.ZoneId == zoneId.Value) &&
            (string.IsNullOrEmpty(assignedTo) || x.AssignedTo == assignedTo) &&
            (!fromDate.HasValue || x.OccurredAt >= fromDate.Value) &&
            (!toDate.HasValue || x.OccurredAt <= toDate.Value))
    {
        ApplyOrderByDescending(x => x.OccurredAt);
    }
}