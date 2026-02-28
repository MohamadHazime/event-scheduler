using EventScheduler.Application.Common;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Domain.Events.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Queries;

public record GetEventAttendeesQuery(Guid EventId) : IRequest<Result<List<AttendeeResponse>>>;

public class GetEventAttendeesQueryHandler : IRequestHandler<GetEventAttendeesQuery, Result<List<AttendeeResponse>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAttendeeRepository _attendeeRepository;

    public GetEventAttendeesQueryHandler(IEventRepository eventRepository, IEventAttendeeRepository attendeeRepository)
    {
        _eventRepository = eventRepository;
        _attendeeRepository = attendeeRepository;
    }

    public async Task<Result<List<AttendeeResponse>>> Handle(GetEventAttendeesQuery request, CancellationToken cancellationToken)
    {
        var evt = await _eventRepository.GetByIdAsync(request.EventId);
        if (evt is null)
            return Result<List<AttendeeResponse>>.Failure("Event not found.", 404);

        var attendees = await _attendeeRepository.GetByEventIdAsync(request.EventId);

        var response = attendees.Select(a => new AttendeeResponse(
            a.UserId,
            a.User?.FullName ?? string.Empty,
            a.User?.Email ?? string.Empty,
            a.Status.ToString()
        )).ToList();

        return Result<List<AttendeeResponse>>.Success(response);
    }
}