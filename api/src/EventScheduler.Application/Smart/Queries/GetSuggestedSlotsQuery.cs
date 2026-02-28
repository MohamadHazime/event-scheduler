using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Smart.DTOs;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Services;
using MediatR;

namespace EventScheduler.Application.Smart.Queries;

public record GetSuggestedSlotsQuery(int DurationMinutes = 60, int Days = 7) : IRequest<Result<List<SlotSuggestion>>>;

public class GetSuggestedSlotsQueryHandler : IRequestHandler<GetSuggestedSlotsQuery, Result<List<SlotSuggestion>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ICurrentUserService _currentUser;

    public GetSuggestedSlotsQueryHandler(IEventRepository eventRepository, ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<SlotSuggestion>>> Handle(GetSuggestedSlotsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<List<SlotSuggestion>>.Failure("Unauthorized.", 401);

        var searchStart = DateTime.UtcNow;
        var searchEnd = searchStart.AddDays(request.Days);

        var existingEvents = await _eventRepository.GetUserAttendingEventsInRangeAsync(
            userId.Value, searchStart, searchEnd);

        var service = new SchedulingSuggestionService();
        var slots = service.SuggestSlots(existingEvents, request.DurationMinutes, searchStart, searchEnd);

        var suggestions = slots.Select(s => new SlotSuggestion(
            s.Start,
            s.End,
            FormatSlotLabel(s.Start, s.End)
        )).ToList();

        return Result<List<SlotSuggestion>>.Success(suggestions);
    }

    private static string FormatSlotLabel(DateTime start, DateTime end)
    {
        if (start.Date == DateTime.UtcNow.Date)
            return $"Today {start:HH:mm} - {end:HH:mm}";
        if (start.Date == DateTime.UtcNow.Date.AddDays(1))
            return $"Tomorrow {start:HH:mm} - {end:HH:mm}";
        return $"{start:ddd MMM dd} {start:HH:mm} - {end:HH:mm}";
    }
}