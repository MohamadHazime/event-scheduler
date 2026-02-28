using EventScheduler.Domain.Common;
using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Invitations;
using EventScheduler.Domain.Users;

namespace EventScheduler.Domain.Events;

public class Event : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Category { get; set; } = EventCategory.Other.Value;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public ICollection<EventAttendee> Attendees { get; set; } = [];
    public ICollection<Invitation> Invitations { get; set; } = [];

    public bool IsOwner(Guid userId) => CreatedById == userId;

    public bool HasTimeConflictWith(Event other)
    {
        var thisRange = new DateTimeRange(StartDate, EndDate);
        var otherRange = new DateTimeRange(other.StartDate, other.EndDate);
        return thisRange.OverlapsWith(otherRange);
    }

    public DateTimeRange GetDateTimeRange() => new(StartDate, EndDate);
}