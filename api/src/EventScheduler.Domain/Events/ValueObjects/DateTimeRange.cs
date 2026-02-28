namespace EventScheduler.Domain.Events.ValueObjects;

public class DateTimeRange : IEquatable<DateTimeRange>
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateTimeRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be after start date.");

        Start = start;
        End = end;
    }

    public bool OverlapsWith(DateTimeRange other)
    {
        return Start < other.End && End > other.Start;
    }

    public TimeSpan Duration => End - Start;

    public bool Equals(DateTimeRange? other) => other is not null && Start == other.Start && End == other.End;
    public override bool Equals(object? obj) => Equals(obj as DateTimeRange);
    public override int GetHashCode() => HashCode.Combine(Start, End);
}