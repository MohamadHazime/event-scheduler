using EventScheduler.Domain.Invitations;
using EventScheduler.Domain.Invitations.Interfaces;
using EventScheduler.Domain.Invitations.ValueObjects;
using EventScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventScheduler.Infrastructure.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly AppDbContext _context;

    public InvitationRepository(AppDbContext context) => _context = context;

    public async Task<Invitation?> GetByTokenAsync(string token)
        => await _context.Invitations
            .Include(i => i.Event)
            .Include(i => i.InvitedBy)
            .FirstOrDefaultAsync(i => i.Token == token);

    public async Task<List<Invitation>> GetByEventIdAsync(Guid eventId)
        => await _context.Invitations
            .Include(i => i.InvitedBy)
            .Where(i => i.EventId == eventId)
            .ToListAsync();

    public async Task<List<Invitation>> GetPendingByEmailAsync(string email)
        => await _context.Invitations
            .Include(i => i.Event)
            .Include(i => i.InvitedBy)
            .Where(i => i.InviteeEmail == email && i.Status == InvitationStatus.Pending)
            .ToListAsync();

    public async Task<List<Invitation>> GetSentByUserAsync(Guid userId)
        => await _context.Invitations
            .Include(i => i.Event)
            .Where(i => i.InvitedById == userId)
            .ToListAsync();

    public async Task AddAsync(Invitation invitation)
        => await _context.Invitations.AddAsync(invitation);

    public void Update(Invitation invitation)
        => _context.Invitations.Update(invitation);
}