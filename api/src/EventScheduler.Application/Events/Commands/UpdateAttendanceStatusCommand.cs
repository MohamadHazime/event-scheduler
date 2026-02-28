using EventScheduler.Application.Common;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Commands;

public record UpdateAttendanceStatusCommand(Guid EventId, string Status) : IRequest<Result<AttendeeResponse>>;

public class UpdateAttendanceStatusCommandHandler : IRequestHandler<UpdateAttendanceStatusCommand, Result<AttendeeResponse>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAttendeeRepository _attendeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateAttendanceStatusCommandHandler(
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

    public async Task<Result<AttendeeResponse>> Handle(UpdateAttendanceStatusCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<AttendeeResponse>.Failure("Unauthorized.", 401);

        var evt = await _eventRepository.GetByIdAsync(command.EventId);
        if (evt is null)
            return Result<AttendeeResponse>.Failure("Event not found.", 404);

        if (!Enum.TryParse<AttendanceStatus>(command.Status, true, out var status))
            return Result<AttendeeResponse>.Failure("Invalid status. Valid values: Upcoming, Attending, Maybe, Declined.");

        var attendee = await _attendeeRepository.GetAsync(command.EventId, userId.Value);

        if (attendee is null)
        {
            attendee = new EventAttendee
            {
                EventId = command.EventId,
                UserId = userId.Value,
                Status = status
            };
            await _attendeeRepository.AddAsync(attendee);
        }
        else
        {
            attendee.Status = status;
            _attendeeRepository.Update(attendee);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _attendeeRepository.GetAsync(command.EventId, userId.Value);

        return Result<AttendeeResponse>.Success(new AttendeeResponse(
            updated!.UserId,
            updated.User?.FullName ?? string.Empty,
            updated.User?.Email ?? string.Empty,
            updated.Status.ToString()));
    }
}