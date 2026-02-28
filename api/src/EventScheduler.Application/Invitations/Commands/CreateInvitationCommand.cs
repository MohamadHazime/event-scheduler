using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Interfaces;
using EventScheduler.Domain.Invitations;
using EventScheduler.Domain.Invitations.Interfaces;
using EventScheduler.Domain.Invitations.ValueObjects;
using EventScheduler.Domain.Users.Interfaces;
using MediatR;

namespace EventScheduler.Application.Invitations.Commands;

public record CreateInvitationCommand(CreateInvitationRequest Request) : IRequest<Result<InvitationResponse>>;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<InvitationResponse>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateInvitationCommandHandler(
        IEventRepository eventRepository,
        IInvitationRepository invitationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<InvitationResponse>> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<InvitationResponse>.Failure("Unauthorized.", 401);

        var request = command.Request;

        var evt = await _eventRepository.GetByIdAsync(request.EventId);
        if (evt is null)
            return Result<InvitationResponse>.Failure("Event not found.", 404);

        if (!evt.IsOwner(userId.Value))
            return Result<InvitationResponse>.Failure("Only the event owner can send invitations.", 403);

        var inviteeEmail = request.InviteeEmail.ToLower().Trim();

        var existingInvitations = await _invitationRepository.GetByEventIdAsync(request.EventId);
        if (existingInvitations.Any(i => i.InviteeEmail == inviteeEmail && i.Status == InvitationStatus.Pending))
            return Result<InvitationResponse>.Failure("An invitation has already been sent to this email for this event.");

        var inviter = await _userRepository.GetByIdAsync(userId.Value);

        var invitation = new Invitation
        {
            EventId = request.EventId,
            InvitedById = userId.Value,
            InviteeEmail = inviteeEmail,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _invitationRepository.AddAsync(invitation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<InvitationResponse>.Success(new InvitationResponse(
            invitation.Id,
            invitation.EventId,
            evt.Title,
            inviter?.FullName ?? string.Empty,
            invitation.InviteeEmail,
            invitation.Token,
            invitation.Status.ToString(),
            invitation.CreatedAt,
            invitation.ExpiresAt), 201);
    }
}