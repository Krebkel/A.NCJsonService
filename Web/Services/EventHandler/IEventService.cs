using Models.EventHandler;

namespace Web.Services.EventHandler;

public interface IEventService
{
    Task AddEventAsync(Event eventItem);
    Task<Dictionary<DateTime, int>> GetEventsSummaryAsync(DateTime startTime, DateTime endTime);
}
