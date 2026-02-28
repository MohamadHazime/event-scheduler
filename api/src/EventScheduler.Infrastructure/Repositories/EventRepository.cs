using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventScheduler.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context) => _context = context;

    public async Task<Event?> GetByIdAsync(Guid id)
        => await _context.Events.Include(e => e.CreatedBy).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Event?> GetByIdWithAttendeesAsync(Guid id)
        => await _context.Events
            .Include(e => e.CreatedBy)
            .Include(e => e.Attendees).ThenInclude(a => a.User)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<Event>> GetEventsByUserAsync(Guid userId, string? title, string? location, string? category, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
    {
        var query = BuildUserEventsQuery(userId, title, location, category, fromDate, toDate);
        return await query
            .OrderBy(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(e => e.CreatedBy)
            .Include(e => e.Attendees)
            .ToListAsync();
    }

    public async Task<int> CountEventsByUserAsync(Guid userId, string? title, string? location, string? category, DateTime? fromDate, DateTime? toDate)
    {
        var query = BuildUserEventsQuery(userId, title, location, category, fromDate, toDate);
        return await query.CountAsync();
    }

    public async Task<List<Event>> GetUserAttendingEventsInRangeAsync(Guid userId, DateTime start, DateTime end)
    {
        return await _context.Events
            .Where(e => e.Attendees.Any(a => a.UserId == userId) || e.CreatedById == userId)
            .Where(e => e.StartDate < end && e.EndDate > start)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task AddAsync(Event evt) => await _context.Events.AddAsync(evt);
    public void Update(Event evt) => _context.Events.Update(evt);
    public void Delete(Event evt) => _context.Events.Remove(evt);

    private IQueryable<Event> BuildUserEventsQuery(Guid userId, string? title, string? location, string? category, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Events
            .Where(e => e.CreatedById == userId || e.Attendees.Any(a => a.UserId == userId));

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(e => e.Title.ToLower().Contains(title.ToLower()));

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(e => e.Location.ToLower().Contains(location.ToLower()));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(e => e.Category == category);

        if (fromDate.HasValue)
            query = query.Where(e => e.StartDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.EndDate <= toDate.Value);

        return query;
    }
}