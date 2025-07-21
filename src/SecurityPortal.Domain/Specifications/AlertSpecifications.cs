using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SecurityPortal.Domain.Specifications;

public static class AlertSpecifications
{
    public static Expression<Func<SecurityAlert, bool>> ByStatus(AlertStatus status)
    {
        return alert => alert.Status == status;
    }

    public static Expression<Func<SecurityAlert, bool>> ByPriority(PriorityLevel priority)
    {
        return alert => alert.Priority.Level == priority;
    }

    public static Expression<Func<SecurityAlert, bool>> ByPlant(Guid plantId)
    {
        return alert => alert.PlantId == plantId;
    }

    public static Expression<Func<SecurityAlert, bool>> ByZone(Guid zoneId)
    {
        return alert => alert.ZoneId == zoneId;
    }

    public static Expression<Func<SecurityAlert, bool>> ByType(AlertType type)
    {
        return alert => alert.Type == type;
    }

    public static Expression<Func<SecurityAlert, bool>> InDateRange(DateTime from, DateTime to)
    {
        return alert => alert.TriggeredAt >= from && alert.TriggeredAt <= to;
    }

    public static Expression<Func<SecurityAlert, bool>> IsActive()
    {
        return alert => alert.Status == AlertStatus.Active || 
                       alert.Status == AlertStatus.Acknowledged ||
                       alert.Status == AlertStatus.InProgress;
    }

    public static Expression<Func<SecurityAlert, bool>> IsUnacknowledged()
    {
        return alert => alert.Status == AlertStatus.Active && alert.AcknowledgedAt == null;
    }

    public static Expression<Func<SecurityAlert, bool>> IsOverdue()
    {
        var cutoffTime = DateTime.UtcNow.AddHours(-4); // 4 hours overdue
        return alert => alert.TriggeredAt < cutoffTime && 
                       alert.Status == AlertStatus.Active;
    }

    public static Expression<Func<SecurityAlert, bool>> HasHighPriority()
    {
        return alert => alert.Priority.Level == PriorityLevel.High || 
                       alert.Priority.Level == PriorityLevel.Critical;
    }

    public static Expression<Func<SecurityAlert, bool>> ContainsText(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        return alert => alert.Title.ToLower().Contains(lowerSearchTerm) ||
                       alert.Message.ToLower().Contains(lowerSearchTerm);
    }

    public static Expression<Func<SecurityAlert, bool>> HasRelatedIncident()
    {
        return alert => alert.RelatedIncidentId != null;
    }
}