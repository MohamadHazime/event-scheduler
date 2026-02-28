using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Application.Smart.DTOs;
using EventScheduler.Domain.EventAttendees;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Events.ValueObjects;
using MediatR;

namespace EventScheduler.Application.Smart.Queries;

public record GetAnalyticsQuery : IRequest<Result<AnalyticsResponse>>;

public class GetAnalyticsQueryHandler : IRequestHandler<GetAnalyticsQuery, Result<AnalyticsResponse>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAttendeeRepository _attendeeRepository;
    private readonly ICurrentUserService _currentUser;

    public GetAnalyticsQueryHandler(
        IEventRepository eventRepository,
        IEventAttendeeRepository attendeeRepository,
        ICurrentUserService currentUser)
    {
        _eventRepository = eventRepository;
        _attendeeRepository = attendeeRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<AnalyticsResponse>> Handle(GetAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<AnalyticsResponse>.Failure("Unauthorized.", 401);

        var allEvents = await _eventRepository.GetUserAttendingEventsInRangeAsync(
            userId.Value,
            DateTime.UtcNow.AddYears(-1),
            DateTime.UtcNow.AddYears(1));

        var upcomingCount = allEvents.Count(e => e.StartDate > DateTime.UtcNow);

        var byCategory = allEvents
            .GroupBy(e => e.Category)
            .Select(g => new CategoryCount(g.Key, g.Count()))
            .OrderByDescending(c => c.Count)
            .ToList();

        var byMonth = allEvents
            .GroupBy(e => e.StartDate.ToString("yyyy-MM"))
            .Select(g => new MonthCount(g.Key, g.Count()))
            .OrderBy(m => m.Month)
            .ToList();

        var busiestDays = allEvents
            .GroupBy(e => e.StartDate.DayOfWeek.ToString())
            .Select(g => new DayCount(g.Key, g.Count()))
            .OrderByDescending(d => d.Count)
            .ToList();

        var allAttendances = new List<EventAttendee>();
        foreach (var evt in allEvents)
        {
            var attendees = await _attendeeRepository.GetByEventIdAsync(evt.Id);
            var userAttendee = attendees.FirstOrDefault(a => a.UserId == userId.Value);
            if (userAttendee is not null)
                allAttendances.Add(userAttendee);
        }

        var attendanceStats = new AttendanceStats(
            allAttendances.Count(a => a.Status == AttendanceStatus.Attending),
            allAttendances.Count(a => a.Status == AttendanceStatus.Maybe),
            allAttendances.Count(a => a.Status == AttendanceStatus.Declined),
            allAttendances.Count(a => a.Status == AttendanceStatus.Upcoming));

        var response = new AnalyticsResponse(
            allEvents.Count,
            upcomingCount,
            byCategory,
            byMonth,
            busiestDays,
            attendanceStats);

        return Result<AnalyticsResponse>.Success(response);
    }
}