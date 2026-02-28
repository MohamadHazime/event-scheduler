using EventScheduler.Application.Auth.Commands;
using EventScheduler.Application.Auth.Queries;
using EventScheduler.Application.Common;
using MediatR;

namespace EventScheduler.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register).AllowAnonymous();
        group.MapPost("/login", Login).AllowAnonymous();
        group.MapGet("/me", GetMe).RequireAuthorization();
    }

    private static async Task<IResult> Register(RegisterCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        return ToResponse(result);
    }

    private static async Task<IResult> Login(LoginCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        return ToResponse(result);
    }

    private static async Task<IResult> GetMe(IMediator mediator)
    {
        var result = await mediator.Send(new GetCurrentUserQuery());
        return ToResponse(result);
    }

    private static IResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Json(result.Data, statusCode: result.StatusCode);

        return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);
    }
}