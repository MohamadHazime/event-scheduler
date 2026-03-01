namespace EventScheduler.Application.Ai.DTOs;

public record ParseEventResult(
    string Title,
    string Description,
    string StartDate,
    string EndDate,
    string Location,
    string Category,
    bool Parsed);