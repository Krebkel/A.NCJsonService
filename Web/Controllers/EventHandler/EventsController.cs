using Microsoft.AspNetCore.Mvc;
using Models.EventHandler;
using Web.Services.EventHandler;
using System.Globalization;

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
        return Ok(summary);
    }
}