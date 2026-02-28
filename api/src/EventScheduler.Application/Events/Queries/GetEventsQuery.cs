using EventScheduler.Application.Common;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Queries;

public record GetEventsQuery(EventListQuery Query) : IRequest<Result<PagedResult<EventResponse>>>;

public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, Result<PagedResult<EventResponse>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEventsQueryHandler(IEventRepository eventRepository, ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<EventResponse>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<PagedResult<EventResponse>>.Failure("Unauthorized.", 401);

        var query = request.Query;

        var events = await _eventRepository.GetEventsByUserAsync(
            userId.Value, query.Title, query.Location, query.Category,
            query.FromDate, query.ToDate, query.Page, query.PageSize);

        var totalCount = await _eventRepository.CountEventsByUserAsync(
            userId.Value, query.Title, query.Location, query.Category,
            query.FromDate, query.ToDate);

        var result = new PagedResult<EventResponse>
        {
            Items = events.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        return Result<PagedResult<EventResponse>>.Success(result);
    }

    private static EventResponse MapToResponse(Event evt)
    {
        return new EventResponse(
            evt.Id,
            evt.Title,
            evt.Description,
            evt.StartDate,
            evt.EndDate,
            evt.Location,
            evt.Category,
            evt.CreatedById,
            evt.CreatedBy?.FullName ?? string.Empty,
            evt.CreatedAt,
            evt.UpdatedAt,
            evt.Attendees.Select(a => new AttendeeResponse(
                a.UserId,
                a.User?.FullName ?? string.Empty,
                a.User?.Email ?? string.Empty,
                a.Status.ToString()
            )).ToList());
    }
}