using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Domain.Invitations.Interfaces;
using MediatR;

namespace EventScheduler.Application.Invitations.Queries;

public record GetSentInvitationsQuery : IRequest<Result<List<InvitationResponse>>>;

public class GetSentInvitationsQueryHandler : IRequestHandler<GetSentInvitationsQuery, Result<List<InvitationResponse>>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetSentInvitationsQueryHandler(IInvitationRepository invitationRepository, ICurrentUserService currentUser)
    {
        _invitationRepository = invitationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<InvitationResponse>>> Handle(GetSentInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<List<InvitationResponse>>.Failure("Unauthorized.", 401);

        var invitations = await _invitationRepository.GetSentByUserAsync(userId.Value);

        var response = invitations.Select(i => new InvitationResponse(
            i.Id,
            i.EventId,
            i.Event?.Title ?? string.Empty,
            i.InvitedBy?.FullName ?? string.Empty,
            i.InviteeEmail,
            i.Token,
            i.Status.ToString(),
            i.CreatedAt,
            i.ExpiresAt
        )).ToList();

        return Result<List<InvitationResponse>>.Success(response);
    }
}