using EventScheduler.Application.Common;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Commands;

public record CreateEventCommand(CreateEventRequest Request) : IRequest<Result<EventResponse>>;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<EventResponse>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAttendeeRepository _attendeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateEventCommandHandler(
        IEventRepository eventRepository,
        IEventAttendeeRepository attendeeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _attendeeRepository = attendeeRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<EventResponse>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<EventResponse>.Failure("Unauthorized.", 401);

        var request = command.Request;

        if (request.EndDate <= request.StartDate)
            return Result<EventResponse>.Failure("End date must be after start date.");

        var evt = new Event
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location?.Trim() ?? string.Empty,
            Category = EventCategory.From(request.Category ?? "Other").Value,
            CreatedById = userId.Value
        };

        await _eventRepository.AddAsync(evt);

        var attendee = new EventAttendee
        {
            EventId = evt.Id,
            UserId = userId.Value,
            Status = AttendanceStatus.Attending
        };

        await _attendeeRepository.AddAsync(attendee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _eventRepository.GetByIdWithAttendeesAsync(evt.Id);
        return Result<EventResponse>.Success(MapToResponse(created!), 201);
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