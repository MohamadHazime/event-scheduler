using EventScheduler.Application.Common;
using EventScheduler.Application.Smart.Commands;
using EventScheduler.Application.Smart.DTOs;
using EventScheduler.Application.Smart.Queries;
using MediatR;

namespace EventScheduler.API.Endpoints;

public static class SmartEndpoints
{
    public static void MapSmartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/smart").WithTags("Smart AI").RequireAuthorization();

        group.MapPost("/check-conflicts", CheckConflicts);
        group.MapPost("/categorize", Categorize);
        group.MapGet("/suggest-slots", SuggestSlots);
        group.MapGet("/analytics", GetAnalytics);
    }

    private static async Task<IResult> CheckConflicts(ConflictCheckRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new CheckConflictsCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> Categorize(CategorizationRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new CategorizeCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> SuggestSlots(int? duration, int? days, IMediator mediator)
    {
        var result = await mediator.Send(new GetSuggestedSlotsQuery(duration ?? 60, days ?? 7));
        return ToResponse(result);
    }

    private static async Task<IResult> GetAnalytics(IMediator mediator)
    {
        var result = await mediator.Send(new GetAnalyticsQuery());
        return ToResponse(result);
    }

    private static IResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Json(result.Data, statusCode: result.StatusCode);

        return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);
    }
}