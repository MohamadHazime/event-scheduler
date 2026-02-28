namespace EventScheduler.Application.Invitations.DTOs;

public record InvitationResponse(
    Guid Id,
    Guid EventId,
    string EventTitle,
    string InvitedByName,
    string InviteeEmail,
    string Token,
    string Status,
    DateTime CreatedAt,
    DateTime ExpiresAt);