namespace EventScheduler.Domain.EventAttendees.Interfaces;

public interface IEventAttendeeRepository
{
    Task<EventAttendee?> GetAsync(Guid eventId, Guid userId);
    Task<List<EventAttendee>> GetByEventIdAsync(Guid eventId);
    Task AddAsync(EventAttendee attendee);
    void Update(EventAttendee attendee);
}