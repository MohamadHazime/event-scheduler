namespace EventScheduler.Application.Events.DTOs;

public record EventResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    string Category,
    Guid CreatedById,
    string CreatedByName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<AttendeeResponse> Attendees);