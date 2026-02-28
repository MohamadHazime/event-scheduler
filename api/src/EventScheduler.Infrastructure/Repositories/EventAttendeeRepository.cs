using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventScheduler.Infrastructure.Repositories;

public class EventAttendeeRepository : IEventAttendeeRepository
{
    private readonly AppDbContext _context;

    public EventAttendeeRepository(AppDbContext context) => _context = context;

    public async Task<EventAttendee?> GetAsync(Guid eventId, Guid userId)
        => await _context.EventAttendees
            .Include(ea => ea.User)
            .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

    public async Task<List<EventAttendee>> GetByEventIdAsync(Guid eventId)
        => await _context.EventAttendees
            .Include(ea => ea.User)
            .Where(ea => ea.EventId == eventId)
            .ToListAsync();

    public async Task AddAsync(EventAttendee attendee)
        => await _context.EventAttendees.AddAsync(attendee);

    public void Update(EventAttendee attendee)
        => _context.EventAttendees.Update(attendee);
}