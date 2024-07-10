using Microsoft.AspNetCore.Mvc;
using Models;
using Web.Services;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly DataService _dataService;

    public PositionsController(DataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Добавление позиции в заказ
    /// </summary>
    [HttpPost("{orderId}/positions")]
    public async Task<ActionResult> AddPositionToOrder(int orderId, [FromBody] Position position)
    {
        if (position.OrderId != orderId)
        {
            return BadRequest("Mismatch between orderId in URL and position data.");
        }
        await _dataService.AddPositionToOrderAsync(position);
        return Ok();
    }

    /// <summary>
    /// Удаление позиции из заказа
    /// </summary>
    [HttpDelete("{orderId}/positions/{wareId}")]
    public async Task<ActionResult> RemovePositionFromOrder(int orderId, int wareId)
    {
        await _dataService.RemovePositionFromOrderAsync(orderId, wareId);
        return Ok();
    }

    /// <summary>
    /// Получение всех позиций по ID заказа
    /// </summary>
    [HttpGet("byOrder/{orderId}")]
    public async Task<ActionResult<IEnumerable<Position>>> GetPositionsByOrderId(int orderId)
    {
        var positions = await _dataService.GetPositionsByOrderIdAsync(orderId);
        if (positions == null || !positions.Any())
        {
            return NotFound();
        }

        return Ok(positions);
    }
}