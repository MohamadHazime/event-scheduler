using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Smart.DTOs;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Services;
using MediatR;

namespace EventScheduler.Application.Smart.Commands;

public record CheckConflictsCommand(ConflictCheckRequest Request) : IRequest<Result<ConflictResult>>;

public class CheckConflictsCommandHandler : IRequestHandler<CheckConflictsCommand, Result<ConflictResult>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ICurrentUserService _currentUser;

    public CheckConflictsCommandHandler(IEventRepository eventRepository, ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<ConflictResult>> Handle(CheckConflictsCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<ConflictResult>.Failure("Unauthorized.", 401);

        var request = command.Request;

        if (request.EndDate <= request.StartDate)
            return Result<ConflictResult>.Failure("End date must be after start date.");

        var existingEvents = await _eventRepository.GetUserAttendingEventsInRangeAsync(
            userId.Value,
            request.StartDate.AddDays(-1),
            request.EndDate.AddDays(1));

        if (request.ExcludeEventId.HasValue)
            existingEvents = existingEvents.Where(e => e.Id != request.ExcludeEventId.Value).ToList();

        var service = new ConflictDetectionService();
        var conflicts = service.DetectConflicts(request.StartDate, request.EndDate, existingEvents);

        var result = new ConflictResult(
            conflicts.Count > 0,
            conflicts.Select(c => new ConflictingEvent(
                c.Id, c.Title, c.StartDate, c.EndDate, c.Location
            )).ToList());

        return Result<ConflictResult>.Success(result);
    }
}