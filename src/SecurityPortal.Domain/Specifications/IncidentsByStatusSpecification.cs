using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Domain.Specifications;

public class IncidentsByStatusSpecification : BaseSpecification<SecurityIncident>
{
    public IncidentsByStatusSpecification(IncidentStatus status) 
        : base(x => x.Status == status)
    {
        ApplyOrderByDescending(x => x.OccurredAt);
    }
}