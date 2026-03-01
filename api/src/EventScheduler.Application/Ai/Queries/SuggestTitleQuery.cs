using EventScheduler.Application.Ai.DTOs;
using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using MediatR;

namespace EventScheduler.Application.Ai.Queries;

public record SuggestTitleQuery(string RoughInput) : IRequest<Result<AiTextResult>>;

public class SuggestTitleQueryHandler : IRequestHandler<SuggestTitleQuery, Result<AiTextResult>>
{
    private readonly IAiService _aiService;

    public SuggestTitleQueryHandler(IAiService aiService)
        => _aiService = aiService;

    public async Task<Result<AiTextResult>> Handle(SuggestTitleQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RoughInput))
            return Result<AiTextResult>.Failure("Input is required.");

        var prompt = $@"Polish the following rough event title into a professional, clear event title. Keep it concise (under 10 words).

            Input: ""{request.RoughInput}""

            Respond with ONLY the polished title, no quotes, no explanation.";

        try
        {
            var response = await _aiService.GenerateContentAsync(prompt);
            return Result<AiTextResult>.Success(new AiTextResult(response.Trim()));
        }
        catch (Exception ex)
        {
            return Result<AiTextResult>.Failure($"AI suggestion failed: {ex.Message}");
        }
    }
}