using EventScheduler.Application.Common;
using EventScheduler.Application.Smart.DTOs;
using EventScheduler.Domain.Events.ValueObjects;
using EventScheduler.Domain.Services;
using MediatR;

namespace EventScheduler.Application.Smart.Commands;

public record CategorizeCommand(CategorizationRequest Request) : IRequest<Result<CategorizationResult>>;

public class CategorizeCommandHandler : IRequestHandler<CategorizeCommand, Result<CategorizationResult>>
{
    public Task<Result<CategorizationResult>> Handle(CategorizeCommand command, CancellationToken cancellationToken)
    {
        var service = new CategorizationService();
        var category = service.Categorize(command.Request.Title, command.Request.Description);

        var result = new CategorizationResult(
            category.Value,
            EventCategory.All.Select(c => c.Value).ToList());

        return Task.FromResult(Result<CategorizationResult>.Success(result));
    }
}