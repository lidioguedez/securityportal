using Microsoft.Extensions.Logging;

namespace SecurityPortal.Infrastructure.BackgroundServices.Common;

/// <summary>
/// Provides scheduling utilities for background services, including CRON-like scheduling
/// </summary>
public static class SchedulingExtensions
{
    /// <summary>
    /// Waits until the next scheduled time based on a CRON-like expression
    /// </summary>
    public static async Task WaitUntilScheduledTime(this CancellationToken cancellationToken, ScheduleOptions schedule, ILogger? logger = null)
    {
        var nextRun = schedule.GetNextRunTime();
        var delay = nextRun - DateTime.UtcNow;
        
        if (delay > TimeSpan.Zero)
        {
            logger?.LogDebug("Next scheduled run at {NextRun} (in {Delay})", nextRun, delay);
            await Task.Delay(delay, cancellationToken);
        }
    }

    /// <summary>
    /// Creates a periodic timer that fires based on the schedule
    /// </summary>
    public static IAsyncEnumerable<DateTime> CreateScheduledTimer(this ScheduleOptions schedule, CancellationToken cancellationToken)
    {
        return CreateScheduledTimerInternal(schedule, cancellationToken);
    }

    private static async IAsyncEnumerable<DateTime> CreateScheduledTimerInternal(
        ScheduleOptions schedule, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var nextRun = schedule.GetNextRunTime();
            var delay = nextRun - DateTime.UtcNow;
            
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken);
            }
            
            yield return DateTime.UtcNow;
        }
    }
}

/// <summary>
/// Represents scheduling options for background services
/// </summary>
public class ScheduleOptions
{
    /// <summary>
    /// The interval between executions
    /// </summary>
    public TimeSpan? Interval { get; set; }

    /// <summary>
    /// Specific times to run (e.g., daily at 2:00 AM)
    /// </summary>
    public List<TimeOnly> DailyTimes { get; set; } = new();

    /// <summary>
    /// Days of week to run (for weekly schedules)
    /// </summary>
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    /// <summary>
    /// Day of month to run (for monthly schedules)
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// Creates a schedule that runs at regular intervals
    /// </summary>
    public static ScheduleOptions Every(TimeSpan interval)
    {
        return new ScheduleOptions { Interval = interval };
    }

    /// <summary>
    /// Creates a schedule that runs daily at specific times
    /// </summary>
    public static ScheduleOptions Daily(params TimeOnly[] times)
    {
        return new ScheduleOptions { DailyTimes = times.ToList() };
    }

    /// <summary>
    /// Creates a schedule that runs on specific days of the week at specific times
    /// </summary>
    public static ScheduleOptions Weekly(IEnumerable<DayOfWeek> daysOfWeek, params TimeOnly[] times)
    {
        return new ScheduleOptions 
        { 
            DaysOfWeek = daysOfWeek.ToList(),
            DailyTimes = times.ToList()
        };
    }

    /// <summary>
    /// Creates a schedule that runs monthly on a specific day at specific times
    /// </summary>
    public static ScheduleOptions Monthly(int dayOfMonth, params TimeOnly[] times)
    {
        return new ScheduleOptions 
        { 
            DayOfMonth = dayOfMonth,
            DailyTimes = times.ToList()
        };
    }

    /// <summary>
    /// Calculates the next run time based on the schedule
    /// </summary>
    public DateTime GetNextRunTime()
    {
        var now = DateTime.UtcNow;

        // Interval-based scheduling
        if (Interval.HasValue)
        {
            return now.Add(Interval.Value);
        }

        // Time-based scheduling
        if (DailyTimes.Any())
        {
            return GetNextTimeBasedRun(now);
        }

        // Default to run immediately
        return now;
    }

    private DateTime GetNextTimeBasedRun(DateTime now)
    {
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        // Check for remaining times today
        var remainingTodayTimes = DailyTimes
            .Where(t => t > currentTime)
            .OrderBy(t => t)
            .ToList();

        // If there are times left today and it's a valid day
        if (remainingTodayTimes.Any() && IsValidDay(now.DayOfWeek, today))
        {
            var nextTime = remainingTodayTimes.First();
            return today.ToDateTime(nextTime, DateTimeKind.Utc);
        }

        // Find next valid day
        for (int i = 1; i <= 31; i++) // Maximum month length
        {
            var nextDay = today.AddDays(i);
            var nextDayOfWeek = nextDay.DayOfWeek;
            
            if (IsValidDay(nextDayOfWeek, nextDay))
            {
                var firstTime = DailyTimes.Min();
                return nextDay.ToDateTime(firstTime, DateTimeKind.Utc);
            }
        }

        // Fallback - run tomorrow at first scheduled time
        var tomorrow = today.AddDays(1);
        var defaultTime = DailyTimes.Any() ? DailyTimes.Min() : new TimeOnly(0, 0);
        return tomorrow.ToDateTime(defaultTime, DateTimeKind.Utc);
    }

    private bool IsValidDay(DayOfWeek dayOfWeek, DateOnly date)
    {
        // Check day of week constraint
        if (DaysOfWeek.Any() && !DaysOfWeek.Contains(dayOfWeek))
        {
            return false;
        }

        // Check day of month constraint
        if (DayOfMonth.HasValue && date.Day != DayOfMonth.Value)
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// Common schedule presets
/// </summary>
public static class SchedulePresets
{
    /// <summary>
    /// Run every minute
    /// </summary>
    public static ScheduleOptions EveryMinute => ScheduleOptions.Every(TimeSpan.FromMinutes(1));

    /// <summary>
    /// Run every 5 minutes
    /// </summary>
    public static ScheduleOptions Every5Minutes => ScheduleOptions.Every(TimeSpan.FromMinutes(5));

    /// <summary>
    /// Run every hour
    /// </summary>
    public static ScheduleOptions Hourly => ScheduleOptions.Every(TimeSpan.FromHours(1));

    /// <summary>
    /// Run daily at 2:00 AM
    /// </summary>
    public static ScheduleOptions DailyAt2AM => ScheduleOptions.Daily(new TimeOnly(2, 0));

    /// <summary>
    /// Run weekdays at 9:00 AM
    /// </summary>
    public static ScheduleOptions WeekdaysAt9AM => ScheduleOptions.Weekly(
        new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
        new TimeOnly(9, 0));

    /// <summary>
    /// Run weekly on Sunday at 3:00 AM
    /// </summary>
    public static ScheduleOptions WeeklySundayAt3AM => ScheduleOptions.Weekly(
        new[] { DayOfWeek.Sunday },
        new TimeOnly(3, 0));

    /// <summary>
    /// Run monthly on the 1st at midnight
    /// </summary>
    public static ScheduleOptions MonthlyFirstAtMidnight => ScheduleOptions.Monthly(1, new TimeOnly(0, 0));
}