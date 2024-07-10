using Microsoft.AspNetCore.Mvc;
using Models.EventHandler;
using Web.Services.EventHandler;
using System.Globalization;
using Newtonsoft.Json;

namespace Web.Controllers.EventHandler;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Добавление события вручную
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostEvent([FromBody] Event eventItem)
    {
        if (eventItem == null)
        {
            return BadRequest();
        }

        await _eventService.AddEventAsync(eventItem);
        return Ok();
    }
    
    /// <summary>
    /// Добавление события через JSON файл
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile importFile)
    {
        if (importFile == null || importFile.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        await _eventService.ImportEventsFromJsonAsync(importFile);
        
        return Ok();
    }

    /// <summary>
    /// Получение данных по событиям
    /// </summary>
    /// <param name="startTime">Время в формате ДД.MM.ГГГГ ЧЧ:ММ:CC</param>
    /// <param name="endTime">Время в формате ДД.MM.ГГГГ ЧЧ:ММ:CC</param>
    [HttpGet]
    public async Task<IActionResult> GetEventSummary([FromQuery] string startTime, [FromQuery] string endTime)
    {
        if (!DateTime.TryParseExact(startTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDateTime))
        {
            return BadRequest("Invalid start time format.");
        }

        if (!DateTime.TryParseExact(endTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDateTime))
        {
            return BadRequest("Invalid end time format.");
        }

        var summary = await _eventService.GetEventsSummaryAsync(startDateTime, endDateTime);
        
        var formattedSummary = summary.ToDictionary(
            keyValuePair => keyValuePair.Key.ToString("dd.MM.yyyy HH:mm:ss"), 
            keyValuePair => keyValuePair.Value
        );

        return Ok(formattedSummary);
    }
}