using EventScheduler.Application.Common;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Domain.Interfaces;
using EventScheduler.Domain.Invitations.Interfaces;
using MediatR;

namespace EventScheduler.Application.Invitations.Commands;

public record DeclineInvitationCommand(string Token) : IRequest<Result<InvitationResponse>>;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand, Result<InvitationResponse>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeclineInvitationCommandHandler(IInvitationRepository invitationRepository, IUnitOfWork unitOfWork)
    {
        _invitationRepository = invitationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InvitationResponse>> Handle(DeclineInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(command.Token);
        if (invitation is null)
            return Result<InvitationResponse>.Failure("Invitation not found.", 404);

        invitation.Decline();
        _invitationRepository.Update(invitation);
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