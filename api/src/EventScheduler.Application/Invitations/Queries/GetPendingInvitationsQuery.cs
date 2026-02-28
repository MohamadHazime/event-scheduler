using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Domain.Invitations.Interfaces;
using MediatR;

namespace EventScheduler.Application.Invitations.Queries;

public record GetPendingInvitationsQuery : IRequest<Result<List<InvitationResponse>>>;

public class GetPendingInvitationsQueryHandler : IRequestHandler<GetPendingInvitationsQuery, Result<List<InvitationResponse>>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetPendingInvitationsQueryHandler(IInvitationRepository invitationRepository, ICurrentUserService currentUser)
    {
        _invitationRepository = invitationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<InvitationResponse>>> Handle(GetPendingInvitationsQuery request, CancellationToken cancellationToken)
    {
        var email = _currentUser.Email;
        if (email is null)
            return Result<List<InvitationResponse>>.Failure("Unauthorized.", 401);

        var invitations = await _invitationRepository.GetPendingByEmailAsync(email);

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