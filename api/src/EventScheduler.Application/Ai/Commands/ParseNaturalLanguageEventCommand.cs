using EventScheduler.Application.Ai.DTOs;
using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using MediatR;
using System.Text.Json;

namespace EventScheduler.Application.Ai.Commands;

public record ParseNaturalLanguageEventCommand(ParseEventRequest Request) : IRequest<Result<ParseEventResult>>;

public class ParseNaturalLanguageEventCommandHandler : IRequestHandler<ParseNaturalLanguageEventCommand, Result<ParseEventResult>>
{
    private readonly IAiService _aiService;

    public ParseNaturalLanguageEventCommandHandler(IAiService aiService)
        => _aiService = aiService;

    public async Task<Result<ParseEventResult>> Handle(ParseNaturalLanguageEventCommand command, CancellationToken cancellationToken)
    {
        var input = command.Request.Input;
        if (string.IsNullOrWhiteSpace(input))
            return Result<ParseEventResult>.Failure("Input cannot be empty.");

        var today = DateTime.UtcNow;
        var prompt = $@"You are an event parser. Parse the following natural language input into a structured event.
            Today's date is {today:yyyy-MM-dd} ({today:dddd}). Use this to resolve relative dates like ""next Friday"", ""tomorrow"", etc.

            Input: ""{input}""

            Respond ONLY with valid JSON in this exact format, no markdown, no code fences:
            {{""title"":""Event title"",""description"":""Brief description of the event"",""startDate"":""yyyy-MM-ddTHH:mm:ss"",""endDate"":""yyyy-MM-ddTHH:mm:ss"",""location"":""Location or empty string"",""category"":""Meeting|Social|Health|Work|Personal|Other""}}

            If the end time is not specified, assume the event lasts 1 hour.
            If the time is not specified, assume 09:00.
            If the location is not mentioned, use an empty string.";

        try
        {
            var response = await _aiService.GenerateContentAsync(prompt);
            var cleaned = response.Trim();

            var codeFence = new string('`', 3);

            if (cleaned.Contains(codeFence))
            {
                var lines = cleaned.Split('\n');
                var jsonLines = new List<string>();
                var insideBlock = false;

                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith(codeFence))
                    {
                        insideBlock = !insideBlock;
                        continue;
                    }
                    if (insideBlock)
                    {
                        jsonLines.Add(line);
                    }
                }

                cleaned = string.Join("\n", jsonLines);
            }

            var parsed = JsonSerializer.Deserialize<ParseEventJson>(cleaned, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (parsed is null)
                return Result<ParseEventResult>.Failure("Failed to parse AI response.");

            return Result<ParseEventResult>.Success(new ParseEventResult(
                parsed.Title ?? "Untitled Event",
                parsed.Description ?? "",
                parsed.StartDate ?? today.ToString("yyyy-MM-ddTHH:mm:ss"),
                parsed.EndDate ?? today.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                parsed.Location ?? "",
                parsed.Category ?? "Other",
                true));
        }
        catch (Exception ex)
        {
            return Result<ParseEventResult>.Failure($"AI parsing failed: {ex.Message}");
        }
    }

    private class ParseEventJson
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }
    }
}
