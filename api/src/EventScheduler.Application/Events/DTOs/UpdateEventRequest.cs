namespace EventScheduler.Application.Events.DTOs;

public record UpdateEventRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    string? Category);