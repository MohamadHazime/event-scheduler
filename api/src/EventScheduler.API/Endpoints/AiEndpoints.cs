using EventScheduler.Application.Ai.Commands;
using EventScheduler.Application.Ai.DTOs;
using EventScheduler.Application.Ai.Queries;
using EventScheduler.Application.Common;
using EventScheduler.Application.Smart.DTOs;
using MediatR;

namespace EventScheduler.API.Endpoints;

public static class AiEndpoints
{
    public static void MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ai").WithTags("AI Features").RequireAuthorization();

        group.MapPost("/parse-event", ParseEvent);
        group.MapPost("/generate-description", GenerateDescription);
        group.MapPost("/categorize", AiCategorize);
        group.MapPost("/suggest-title", SuggestTitle);
    }

    private static async Task<IResult> ParseEvent(ParseEventRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new ParseNaturalLanguageEventCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> GenerateDescription(GenerateDescriptionRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new GenerateDescriptionCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> AiCategorize(CategorizationRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new AiCategorizeCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> SuggestTitle(SuggestTitleRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new SuggestTitleQuery(request.Input));
        return ToResponse(result);
    }

    private static IResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Json(result.Data, statusCode: result.StatusCode);

        return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);
    }
}