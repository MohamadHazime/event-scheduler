using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.ValueObjects;

namespace EventScheduler.Domain.Services;

public class SchedulingSuggestionService
{
    public List<DateTimeRange> SuggestSlots(
        List<Event> existingEvents,
        int durationMinutes,
        DateTime searchStart,
        DateTime searchEnd,
        int startHour = 8,
        int endHour = 20,
        int maxSuggestions = 5)
    {
        var suggestions = new List<DateTimeRange>();
        var duration = TimeSpan.FromMinutes(durationMinutes);

        var sortedEvents = existingEvents
            .OrderBy(e => e.StartDate)
            .ToList();

        var currentDate = searchStart.Date;
        var lastDate = searchEnd.Date;

        while (currentDate <= lastDate && suggestions.Count < maxSuggestions)
        {
            var dayStart = currentDate.AddHours(startHour);
            var dayEnd = currentDate.AddHours(endHour);

            if (dayStart < searchStart)
                dayStart = searchStart;

            var dayEvents = sortedEvents
                .Where(e => e.StartDate.Date == currentDate.Date || e.EndDate.Date == currentDate.Date)
                .Where(e => e.StartDate < dayEnd && e.EndDate > dayStart)
                .OrderBy(e => e.StartDate)
                .ToList();

            var slotStart = dayStart;

            foreach (var evt in dayEvents)
            {
                var gapEnd = evt.StartDate < slotStart ? slotStart : evt.StartDate;

                if (gapEnd - slotStart >= duration && slotStart >= dayStart)
                {
                    suggestions.Add(new DateTimeRange(slotStart, slotStart + duration));
                    if (suggestions.Count >= maxSuggestions) break;
                }

                if (evt.EndDate > slotStart)
                    slotStart = evt.EndDate;
            }

            if (suggestions.Count < maxSuggestions && dayEnd - slotStart >= duration)
            {
                suggestions.Add(new DateTimeRange(slotStart, slotStart + duration));
            }

            currentDate = currentDate.AddDays(1);
        }

        return suggestions;
    }
}