using EventScheduler.Application.Common;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Domain.Events.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Queries;

public record GetEventByIdQuery(Guid Id) : IRequest<Result<EventResponse>>;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, Result<EventResponse>>
{
    private readonly IEventRepository _eventRepository;

    public GetEventByIdQueryHandler(IEventRepository eventRepository)
        => _eventRepository = eventRepository;

    public async Task<Result<EventResponse>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var evt = await _eventRepository.GetByIdWithAttendeesAsync(request.Id);
        if (evt is null)
            return Result<EventResponse>.Failure("Event not found.", 404);

        var response = new EventResponse(
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

        return Result<EventResponse>.Success(response);
    }
}