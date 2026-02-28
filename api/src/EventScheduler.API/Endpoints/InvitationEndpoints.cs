using EventScheduler.Application.Common;
using EventScheduler.Application.Invitations.Commands;
using EventScheduler.Application.Invitations.DTOs;
using EventScheduler.Application.Invitations.Queries;
using MediatR;

namespace EventScheduler.API.Endpoints;

public static class InvitationEndpoints
{
    public static void MapInvitationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invitations").WithTags("Invitations");

        group.MapPost("/", CreateInvitation).RequireAuthorization();
        group.MapGet("/pending", GetPendingInvitations).RequireAuthorization();
        group.MapGet("/sent", GetSentInvitations).RequireAuthorization();
        group.MapGet("/link/{token}", GetByToken);
        group.MapPost("/{token}/accept", AcceptInvitation).RequireAuthorization();
        group.MapPost("/{token}/decline", DeclineInvitation).RequireAuthorization();
    }

    private static async Task<IResult> CreateInvitation(CreateInvitationRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new CreateInvitationCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> GetPendingInvitations(IMediator mediator)
    {
        var result = await mediator.Send(new GetPendingInvitationsQuery());
        return ToResponse(result);
    }

    private static async Task<IResult> GetSentInvitations(IMediator mediator)
    {
        var result = await mediator.Send(new GetSentInvitationsQuery());
        return ToResponse(result);
    }

    private static async Task<IResult> GetByToken(string token, IMediator mediator)
    {
        var result = await mediator.Send(new GetInvitationByTokenQuery(token));
        return ToResponse(result);
    }

    private static async Task<IResult> AcceptInvitation(string token, IMediator mediator)
    {
        var result = await mediator.Send(new AcceptInvitationCommand(token));
        return ToResponse(result);
    }

    private static async Task<IResult> DeclineInvitation(string token, IMediator mediator)
    {
        var result = await mediator.Send(new DeclineInvitationCommand(token));
        return ToResponse(result);
    }

    private static IResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Json(result.Data, statusCode: result.StatusCode);

        return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);
    }
}