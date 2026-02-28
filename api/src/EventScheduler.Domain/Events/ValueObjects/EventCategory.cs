namespace EventScheduler.Domain.Events.ValueObjects;

public class EventCategory : IEquatable<EventCategory>
{
    public string Value { get; }

    private EventCategory(string value) => Value = value;

    public static readonly EventCategory Meeting = new("Meeting");
    public static readonly EventCategory Social = new("Social");
    public static readonly EventCategory Health = new("Health");
    public static readonly EventCategory Work = new("Work");
    public static readonly EventCategory Personal = new("Personal");
    public static readonly EventCategory Other = new("Other");

    private static readonly List<EventCategory> _all = [Meeting, Social, Health, Work, Personal, Other];

    public static EventCategory From(string value)
    {
        var category = _all.FirstOrDefault(c => c.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        return category ?? Other;
    }

    public static IReadOnlyList<EventCategory> All => _all.AsReadOnly();

    public bool Equals(EventCategory? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as EventCategory);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}