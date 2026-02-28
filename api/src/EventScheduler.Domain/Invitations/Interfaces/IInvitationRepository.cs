namespace EventScheduler.Domain.Invitations.Interfaces;

public interface IInvitationRepository
{
    Task<Invitation?> GetByTokenAsync(string token);
    Task<List<Invitation>> GetByEventIdAsync(Guid eventId);
    Task<List<Invitation>> GetPendingByEmailAsync(string email);
    Task<List<Invitation>> GetSentByUserAsync(Guid userId);
    Task AddAsync(Invitation invitation);
    void Update(Invitation invitation);
}