namespace EventScheduler.Application.Smart.DTOs;

public record ConflictCheckRequest(DateTime StartDate, DateTime EndDate, Guid? ExcludeEventId = null);