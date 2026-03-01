using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Smart.DTOs;
using EventScheduler.Domain.Events.ValueObjects;
using MediatR;

namespace EventScheduler.Application.Ai.Commands;

public record AiCategorizeCommand(CategorizationRequest Request) : IRequest<Result<CategorizationResult>>;

public class AiCategorizeCommandHandler : IRequestHandler<AiCategorizeCommand, Result<CategorizationResult>>
{
    private readonly IAiService _aiService;

    public AiCategorizeCommandHandler(IAiService aiService)
        => _aiService = aiService;

    public async Task<Result<CategorizationResult>> Handle(AiCategorizeCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (string.IsNullOrWhiteSpace(req.Title))
            return Result<CategorizationResult>.Failure("Title is required.");

        var prompt = $@"Categorize the following event into exactly one of these categories: Meeting, Social, Health, Work, Personal, Other.

            Title: {req.Title}
            Description: {req.Description ?? ""}

            Respond with ONLY the category name, nothing else.";

        try
        {
            var response = await _aiService.GenerateContentAsync(prompt);
            var category = response.Trim();
            var validCategory = EventCategory.From(category);

            return Result<CategorizationResult>.Success(new CategorizationResult(
                validCategory.Value,
                EventCategory.All.Select(c => c.Value).ToList()));
        }
        catch
        {
            var fallback = new Domain.Services.CategorizationService();
            var category = fallback.Categorize(req.Title, req.Description);
            return Result<CategorizationResult>.Success(new CategorizationResult(
                category.Value,
                EventCategory.All.Select(c => c.Value).ToList()));
        }
    }
}