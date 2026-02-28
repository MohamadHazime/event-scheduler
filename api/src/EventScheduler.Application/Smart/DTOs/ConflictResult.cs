namespace EventScheduler.Application.Smart.DTOs;

public record ConflictResult(
    bool HasConflicts,
    List<ConflictingEvent> Conflicts);

public record ConflictingEvent(
    Guid Id,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    string Location);