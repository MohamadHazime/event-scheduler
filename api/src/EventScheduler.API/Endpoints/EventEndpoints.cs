using EventScheduler.Application.Common;
using EventScheduler.Application.Events.Commands;
using EventScheduler.Application.Events.DTOs;
using EventScheduler.Application.Events.Queries;
using MediatR;

namespace EventScheduler.API.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events").RequireAuthorization();

        group.MapGet("/", GetEvents);
        group.MapGet("/{id:guid}", GetEventById);
        group.MapPost("/", CreateEvent);
        group.MapPut("/{id:guid}", UpdateEvent);
        group.MapDelete("/{id:guid}", DeleteEvent);
        group.MapPut("/{id:guid}/status", UpdateStatus);
        group.MapGet("/{id:guid}/attendees", GetAttendees);
    }

    private static async Task<IResult> GetEvents(
        [AsParameters] EventListQuery query,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetEventsQuery(query));
        return ToResponse(result);
    }

    private static async Task<IResult> GetEventById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetEventByIdQuery(id));
        return ToResponse(result);
    }

    private static async Task<IResult> CreateEvent(CreateEventRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new CreateEventCommand(request));
        return ToResponse(result);
    }

    private static async Task<IResult> UpdateEvent(Guid id, UpdateEventRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateEventCommand(id, request));
        return ToResponse(result);
    }

    private static async Task<IResult> DeleteEvent(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteEventCommand(id));
        return ToResponse(result);
    }

    private static async Task<IResult> UpdateStatus(Guid id, UpdateStatusRequest request, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateAttendanceStatusCommand(id, request.Status));
        return ToResponse(result);
    }

    private static async Task<IResult> GetAttendees(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetEventAttendeesQuery(id));
        return ToResponse(result);
    }

    private static IResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Json(result.Data, statusCode: result.StatusCode);

        return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);
    }
}