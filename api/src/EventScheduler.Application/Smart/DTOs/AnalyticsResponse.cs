namespace EventScheduler.Application.Smart.DTOs;

public record AnalyticsResponse(
    int TotalEvents,
    int UpcomingEvents,
    List<CategoryCount> EventsByCategory,
    List<MonthCount> EventsByMonth,
    List<DayCount> BusiestDays,
    AttendanceStats AttendanceStats);

public record CategoryCount(string Category, int Count);
public record MonthCount(string Month, int Count);
public record DayCount(string Day, int Count);
public record AttendanceStats(int Attending, int Maybe, int Declined, int Upcoming);