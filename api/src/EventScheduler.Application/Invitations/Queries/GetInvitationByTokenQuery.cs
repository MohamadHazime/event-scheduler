using EventScheduler.Application.Common;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Domain.Invitations.Interfaces;
using MediatR;

namespace EventScheduler.Application.Invitations.Queries;

public record GetInvitationByTokenQuery(string Token) : IRequest<Result<InvitationResponse>>;

public class GetInvitationByTokenQueryHandler : IRequestHandler<GetInvitationByTokenQuery, Result<InvitationResponse>>
{
    private readonly IInvitationRepository _invitationRepository;

    public GetInvitationByTokenQueryHandler(IInvitationRepository invitationRepository)
        => _invitationRepository = invitationRepository;

    public async Task<Result<InvitationResponse>> Handle(GetInvitationByTokenQuery request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(request.Token);
        if (invitation is null)
            return Result<InvitationResponse>.Failure("Invitation not found.", 404);

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