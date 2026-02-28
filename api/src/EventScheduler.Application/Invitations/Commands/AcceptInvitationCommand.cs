using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Interfaces;
using EventScheduler.Domain.Invitations.Interfaces;
using MediatR;

namespace EventScheduler.Application.Invitations.Commands;

public record AcceptInvitationCommand(string Token) : IRequest<Result<InvitationResponse>>;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result<InvitationResponse>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IEventAttendeeRepository _attendeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AcceptInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IEventAttendeeRepository attendeeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _invitationRepository = invitationRepository;
        _attendeeRepository = attendeeRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<InvitationResponse>> Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<InvitationResponse>.Failure("Unauthorized.", 401);

        var invitation = await _invitationRepository.GetByTokenAsync(command.Token);
        if (invitation is null)
            return Result<InvitationResponse>.Failure("Invitation not found.", 404);

        if (!invitation.CanBeAccepted())
        {
            var reason = invitation.IsExpired() ? "This invitation has expired." : "This invitation is no longer pending.";
            return Result<InvitationResponse>.Failure(reason);
        }

        invitation.Accept();
        _invitationRepository.Update(invitation);

        var existingAttendee = await _attendeeRepository.GetAsync(invitation.EventId, userId.Value);
        if (existingAttendee is null)
        {
            var attendee = new EventAttendee
            {
                EventId = invitation.EventId,
                UserId = userId.Value,
                Status = AttendanceStatus.Attending
            };
            await _attendeeRepository.AddAsync(attendee);
        }
        else
        {
            existingAttendee.Status = AttendanceStatus.Attending;
            _attendeeRepository.Update(existingAttendee);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<InvitationResponse>.Success(new InvitationResponse(
            invitation.Id,
            invitation.EventId,
            invitation.Event?.Title ?? string.Empty,
            invitation.InvitedBy?.FullName ?? string.Empty,
            invitation.InviteeEmail,
            invitation.Token,
            invitation.Status.ToString(),
            invitation.CreatedAt,
            invitation.ExpiresAt));
    }
}