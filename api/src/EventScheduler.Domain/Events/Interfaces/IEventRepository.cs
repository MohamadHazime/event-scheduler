namespace EventScheduler.Domain.Events.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id);
    Task<Event?> GetByIdWithAttendeesAsync(Guid id);
    Task<List<Event>> GetEventsByUserAsync(Guid userId, string? title, string? location, string? category, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
    Task<int> CountEventsByUserAsync(Guid userId, string? title, string? location, string? category, DateTime? fromDate, DateTime? toDate);
    Task<List<Event>> GetUserAttendingEventsInRangeAsync(Guid userId, DateTime start, DateTime end);
    Task AddAsync(Event evt);
    void Update(Event evt);
    void Delete(Event evt);
}