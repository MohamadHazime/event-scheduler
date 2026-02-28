using EventScheduler.Domain.Common;
using EventScheduler.Domain.Events;
using EventScheduler.Domain.Invitations.ValueObjects;
using EventScheduler.Domain.Users;

namespace EventScheduler.Domain.Invitations;

public class Invitation : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid InvitedById { get; set; }
    public User InvitedBy { get; set; } = null!;

    public string InviteeEmail { get; set; } = string.Empty;
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool CanBeAccepted() => Status == InvitationStatus.Pending && !IsExpired();

    public void Accept()
    {
        if (!CanBeAccepted())
            throw new InvalidOperationException("Invitation cannot be accepted.");
        Status = InvitationStatus.Accepted;
    }

    public void Decline()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Only pending invitations can be declined.");
        Status = InvitationStatus.Declined;
    }
}