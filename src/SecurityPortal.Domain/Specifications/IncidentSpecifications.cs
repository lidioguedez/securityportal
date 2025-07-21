using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SecurityPortal.Domain.Specifications;

public static class IncidentSpecifications
{
    public static Expression<Func<SecurityIncident, bool>> ByStatus(IncidentStatus status)
    {
        return incident => incident.Status == status;
    }

    public static Expression<Func<SecurityIncident, bool>> ByPriority(PriorityLevel priority)
    {
        return incident => incident.Priority.Level == priority;
    }

    public static Expression<Func<SecurityIncident, bool>> ByPlant(Guid plantId)
    {
        return incident => incident.PlantId == plantId;
    }

    public static Expression<Func<SecurityIncident, bool>> ByZone(Guid zoneId)
    {
        return incident => incident.ZoneId == zoneId;
    }

    public static Expression<Func<SecurityIncident, bool>> ByAssignee(string assignee)
    {
        return incident => incident.AssignedTo == assignee;
    }

    public static Expression<Func<SecurityIncident, bool>> ByType(IncidentType type)
    {
        return incident => incident.Type == type;
    }

    public static Expression<Func<SecurityIncident, bool>> InDateRange(DateTime from, DateTime to)
    {
        return incident => incident.OccurredAt >= from && incident.OccurredAt <= to;
    }

    public static Expression<Func<SecurityIncident, bool>> IsOpen()
    {
        return incident => incident.Status == IncidentStatus.Open || 
                          incident.Status == IncidentStatus.InvestigationInProgress ||
                          incident.Status == IncidentStatus.UnderReview;
    }

    public static Expression<Func<SecurityIncident, bool>> IsOverdue()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-7); // 7 days overdue
        return incident => incident.CreatedAt < cutoffDate && 
                          (incident.Status == IncidentStatus.Open || 
                           incident.Status == IncidentStatus.InvestigationInProgress);
    }

    public static Expression<Func<SecurityIncident, bool>> HasHighPriority()
    {
        return incident => incident.Priority.Level == PriorityLevel.High || 
                          incident.Priority.Level == PriorityLevel.Critical;
    }

    public static Expression<Func<SecurityIncident, bool>> ContainsText(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        return incident => incident.Title.ToLower().Contains(lowerSearchTerm) ||
                          incident.Description.ToLower().Contains(lowerSearchTerm);
    }
}