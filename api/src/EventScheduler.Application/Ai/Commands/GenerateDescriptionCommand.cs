using EventScheduler.Application.Ai.DTOs;
using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using MediatR;

namespace EventScheduler.Application.Ai.Commands;

public record GenerateDescriptionCommand(GenerateDescriptionRequest Request) : IRequest<Result<AiTextResult>>;

public class GenerateDescriptionCommandHandler : IRequestHandler<GenerateDescriptionCommand, Result<AiTextResult>>
{
    private readonly IAiService _aiService;

    public GenerateDescriptionCommandHandler(IAiService aiService)
        => _aiService = aiService;

    public async Task<Result<AiTextResult>> Handle(GenerateDescriptionCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (string.IsNullOrWhiteSpace(req.Title))
            return Result<AiTextResult>.Failure("Title is required.");

        var prompt = $@"Generate a professional, concise event description (2-3 sentences) for the following event:

            Title: {req.Title}
            Category: {req.Category ?? "General"}
            Location: {req.Location ?? "Not specified"}

            Respond with ONLY the description text, no quotes, no labels, no formatting.";

        try
        {
            var response = await _aiService.GenerateContentAsync(prompt);
            return Result<AiTextResult>.Success(new AiTextResult(response.Trim()));
        }
        catch (Exception ex)
        {
            return Result<AiTextResult>.Failure($"AI generation failed: {ex.Message}");
        }
    }
}