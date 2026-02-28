namespace EventScheduler.Application.Events.DTOs;

public record CreateEventRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    string? Category);