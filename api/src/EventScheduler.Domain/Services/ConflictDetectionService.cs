using EventScheduler.Domain.Events;
using EventScheduler.Domain.Events.ValueObjects;

namespace EventScheduler.Domain.Services;

public class ConflictDetectionService
{
    public List<Event> DetectConflicts(DateTime start, DateTime end, List<Event> existingEvents)
    {
        var range = new DateTimeRange(start, end);

        return existingEvents
            .Where(e =>
            {
                var eventRange = new DateTimeRange(e.StartDate, e.EndDate);
                return range.OverlapsWith(eventRange);
            })
            .ToList();
    }
}