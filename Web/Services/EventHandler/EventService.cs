using Dapper;
using Models.EventHandler;
using Newtonsoft.Json;
using Npgsql;

namespace Web.Services.EventHandler;

public class EventService : IEventService
{
    private readonly NpgsqlConnection _connection;

    public EventService(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Добавление события с некоторым весом в базу данных
    /// </summary>
    public async Task AddEventAsync(Event eventItem)
    {
        eventItem.Timestamp = DateTime.UtcNow; // Время генерируется автоматически
        var query = "INSERT INTO Events (Name, Value, Timestamp) VALUES (@Name, @Value, @Timestamp)";
        await _connection.ExecuteAsync(query, eventItem);
    }
    
    /// <summary>
    /// Импорт события из JSON
    /// </summary>
    public async Task ImportEventsFromJsonAsync(IFormFile importFile)
    {
        if (importFile == null || importFile.Length == 0)
        {
            throw new InvalidDataException("Invalid file.");
        }

        using var stream = new StreamReader(importFile.OpenReadStream());
        var content = await stream.ReadToEndAsync();
        var events = JsonConvert.DeserializeObject<List<Event>>(content);

        foreach (var eventItem in events)
        {
            eventItem.Timestamp = DateTime.UtcNow;
            await AddEventAsync(eventItem);
        }
    }

    /// <summary>
    /// Получение списка Время - Сумма
    /// </summary>
    public async Task<Dictionary<DateTime, int>> GetEventsSummaryAsync(DateTime startTime, DateTime endTime)
    {
        var query = @"
            SELECT date_trunc('minute', Timestamp) as Time, COALESCE(SUM(Value), 0) as Total
            FROM Events
            WHERE Timestamp >= @StartTime AND Timestamp <= @EndTime
            GROUP BY date_trunc('minute', Timestamp)
            ORDER BY Time";

        var result = await _connection.QueryAsync<(DateTime Time, int Total)>(
            query, 
            new { StartTime = startTime, EndTime = endTime }
        );

        var summary = result.ToDictionary(r => r.Time, r => r.Total);

        for (var time = startTime; time <= endTime; time = time.AddMinutes(1))
        {
            if (!summary.ContainsKey(time))
            {
                summary[time] = 0;
            }
        }

        var orderedSummary = summary
            .OrderBy(keyValuePair => keyValuePair.Key)
            .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

        return orderedSummary;
    }
}