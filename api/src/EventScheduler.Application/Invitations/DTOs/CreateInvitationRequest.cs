namespace EventScheduler.Application.Invitations.DTOs;

public record CreateInvitationRequest(Guid EventId, string InviteeEmail);