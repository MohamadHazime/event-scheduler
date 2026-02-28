using EventScheduler.Domain.Events.ValueObjects;

namespace EventScheduler.Domain.Services;

public class CategorizationService
{
    private static readonly Dictionary<string, string[]> _categoryKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Meeting"] = ["meeting", "standup", "sync", "huddle", "1:1", "one-on-one", "retrospective", "retro", "review", "planning", "scrum", "sprint", "demo", "kickoff", "briefing", "debrief"],
        ["Social"] = ["party", "birthday", "celebration", "dinner", "lunch", "brunch", "happy hour", "gathering", "hangout", "get-together", "barbecue", "bbq", "picnic", "wedding", "anniversary"],
        ["Health"] = ["doctor", "dentist", "appointment", "checkup", "therapy", "gym", "workout", "yoga", "meditation", "run", "exercise", "clinic", "hospital", "vaccine", "physical"],
        ["Work"] = ["deadline", "presentation", "conference", "workshop", "training", "seminar", "webinar", "interview", "onboarding", "offsite", "team building", "hackathon", "release", "deployment"],
        ["Personal"] = ["errand", "shopping", "haircut", "bank", "repair", "maintenance", "move", "travel", "vacation", "holiday", "flight", "hotel", "pickup", "drop-off"]
    };

    public EventCategory Categorize(string title, string? description = null)
    {
        var text = $"{title} {description ?? ""}".ToLower();

        var bestMatch = "Other";
        var highestScore = 0;

        foreach (var (category, keywords) in _categoryKeywords)
        {
            var score = keywords.Count(keyword => text.Contains(keyword));
            if (score > highestScore)
            {
                highestScore = score;
                bestMatch = category;
            }
        }

        return EventCategory.From(bestMatch);
    }
}