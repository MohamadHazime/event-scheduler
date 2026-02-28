using EventScheduler.Domain.Common;
using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Users;

namespace EventScheduler.Domain.EventAttendees;

public class EventAttendee : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Upcoming;
}