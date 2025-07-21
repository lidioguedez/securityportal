namespace SecurityPortal.Domain.ValueObjects;

public enum PriorityLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public sealed class Priority : IEquatable<Priority>
{
    public PriorityLevel Level { get; private init; }
    public string Description { get; private init; } = string.Empty;

    private Priority(PriorityLevel level, string description)
    {
        Level = level;
        Description = description;
    }

    public static Priority Low => new(PriorityLevel.Low, "Low Priority");
    public static Priority Medium => new(PriorityLevel.Medium, "Medium Priority");
    public static Priority High => new(PriorityLevel.High, "High Priority");
    public static Priority Critical => new(PriorityLevel.Critical, "Critical Priority");

    public static Priority FromLevel(PriorityLevel level) => level switch
    {
        PriorityLevel.Low => Low,
        PriorityLevel.Medium => Medium,
        PriorityLevel.High => High,
        PriorityLevel.Critical => Critical,
        _ => throw new ArgumentException($"Unknown priority level: {level}")
    };

    public static bool TryParse(string value, out Priority priority)
    {
        priority = Medium;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.ToLowerInvariant() switch
        {
            "low" => SetResult(Low, out priority),
            "medium" => SetResult(Medium, out priority),
            "high" => SetResult(High, out priority),
            "critical" => SetResult(Critical, out priority),
            _ => false
        };
    }

    private static bool SetResult(Priority result, out Priority priority)
    {
        priority = result;
        return true;
    }

    public bool Equals(Priority? other)
    {
        return other is not null && Level == other.Level;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Priority);
    }

    public override int GetHashCode()
    {
        return Level.GetHashCode();
    }

    public static bool operator ==(Priority? left, Priority? right)
    {
        return EqualityComparer<Priority>.Default.Equals(left, right);
    }

    public static bool operator !=(Priority? left, Priority? right)
    {
        return !(left == right);
    }
}