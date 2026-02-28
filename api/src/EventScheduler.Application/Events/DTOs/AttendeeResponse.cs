namespace EventScheduler.Application.Events.DTOs;

public record AttendeeResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Status);