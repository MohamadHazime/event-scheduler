namespace EventScheduler.Application.Events.DTOs;

public record EventListQuery(
    string? Title,
    string? Location,
    string? Category,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 10);