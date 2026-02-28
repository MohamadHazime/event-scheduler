using EventScheduler.Domain.Common;
using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.Events;
using EventScheduler.Domain.Invitations;

namespace EventScheduler.Domain.Users;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public ICollection<Event> CreatedEvents { get; set; } = [];
    public ICollection<EventAttendee> Attendances { get; set; } = [];
    public ICollection<Invitation> SentInvitations { get; set; } = [];
}