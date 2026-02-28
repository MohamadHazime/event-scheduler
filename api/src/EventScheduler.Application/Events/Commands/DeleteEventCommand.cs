using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Interfaces;
using MediatR;

namespace EventScheduler.Application.Events.Commands;

public record DeleteEventCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<bool>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteEventCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(DeleteEventCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<bool>.Failure("Unauthorized.", 401);

        var evt = await _eventRepository.GetByIdAsync(command.Id);
        if (evt is null)
            return Result<bool>.Failure("Event not found.", 404);

        if (!evt.IsOwner(userId.Value))
            return Result<bool>.Failure("Only the event owner can delete this event.", 403);

        _eventRepository.Delete(evt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}