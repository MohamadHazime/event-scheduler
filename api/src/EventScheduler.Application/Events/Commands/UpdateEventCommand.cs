using EventScheduler.Application.Common;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Commands;

public record UpdateEventCommand(Guid Id, UpdateEventRequest Request) : IRequest<Result<EventResponse>>;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result<EventResponse>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateEventCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<EventResponse>> Handle(UpdateEventCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<EventResponse>.Failure("Unauthorized.", 401);

        var evt = await _eventRepository.GetByIdWithAttendeesAsync(command.Id);
        if (evt is null)
            return Result<EventResponse>.Failure("Event not found.", 404);

        if (!evt.IsOwner(userId.Value))
            return Result<EventResponse>.Failure("Only the event owner can update this event.", 403);

        var request = command.Request;

        if (request.EndDate <= request.StartDate)
            return Result<EventResponse>.Failure("End date must be after start date.");

        evt.Title = request.Title.Trim();
        evt.Description = request.Description?.Trim() ?? string.Empty;
        evt.StartDate = request.StartDate;
        evt.EndDate = request.EndDate;
        evt.Location = request.Location?.Trim() ?? string.Empty;
        evt.Category = EventCategory.From(request.Category ?? "Other").Value;

        _eventRepository.Update(evt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _eventRepository.GetByIdWithAttendeesAsync(evt.Id);
        return Result<EventResponse>.Success(MapToResponse(updated!));
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