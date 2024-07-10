using Microsoft.AspNetCore.Mvc;
using Models;
using Web.Services;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly DataService _dataService;

    public OrdersController(DataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Получение всех заказов
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await _dataService.GetOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Получение заказа по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _dataService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    /// <summary>
    /// Создать заказ
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] Order order)
    {
        await _dataService.CreateOrderAsync(order);
        return Ok();
    }

    /// <summary>
    /// Обновить заказ
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateOrder(int id, [FromBody] Order order)
    {
        if (id != order.Id)
        {
            return BadRequest();
        }
        await _dataService.UpdateOrderAsync(order);
        return Ok();
    }

    /// <summary>
    /// Удалить заказ
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        await _dataService.DeleteOrderAsync(id);
        return Ok();
    }

    /// <summary>
    /// Импорт заказов и позиций из JSON
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult> ImportOrders(IFormFile importFile)
    {
        if (importFile == null || importFile.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        try
        {
            await _dataService.ImportOrdersFromJsonAsync(importFile);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Экспорт всех заказов и позиций в JSON
    /// </summary>
    [HttpGet("export")]
    public async Task<ActionResult> ExportOrders()
    {
        var data = await _dataService.ExportOrdersToJsonAsync();
        return File(data, "application/json", "orders.json");
    }

    /// <summary>
    /// Экспорт заказа по ID в JSON
    /// </summary>
    [HttpGet("{id}/export")]
    public async Task<ActionResult> ExportOrderById(int id)
    {
        var data = await _dataService.ExportOrderByIdToJsonAsync(id);
        return File(data, "application/json", $"order_{id}.json");
    }
}
