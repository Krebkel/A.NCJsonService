using Microsoft.AspNetCore.Mvc;
using Models.Accounting;
using Web.Services.Accounting;

namespace Web.Controllers.Accounting;

[ApiController]
[Route("api/[controller]")]
public class WaresController : ControllerBase
{
    private readonly DataService _dataService;

    public WaresController(DataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ware>>> GetWares()
    {
        var wares = await _dataService.GetWaresAsync();
        return Ok(wares);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ware>> GetWare(int id)
    {
        var ware = await _dataService.GetWareByIdAsync(id);
        if (ware == null)
        {
            return NotFound();
        }
        return Ok(ware);
    }

    [HttpPost]
    public async Task<ActionResult<Ware>> CreateWare([FromBody] Ware ware)
    {
        await _dataService.CreateWareAsync(ware);
        return CreatedAtAction(nameof(GetWare), new { id = ware.Id }, ware);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWare(int id, [FromBody] Ware ware)
    {
        if (id != ware.Id)
        {
            return BadRequest();
        }

        await _dataService.UpdateWareAsync(ware);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWare(int id)
    {
        await _dataService.DeleteWareAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Импорт товаров из JSON
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile importFile)
    {
        if (importFile == null || importFile.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        await _dataService.ImportWaresFromJsonAsync(importFile);
        return Ok();
    }

    /// <summary>
    /// Экспорт товаров в JSON
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _dataService.ExportWaresToJsonAsync();
        return File(data, "application/json", "wares.json");
    }
    
    /// <summary>
    /// Экспорт товара по ID в JSON
    /// </summary>
    [HttpGet("{id}/export")]
    public async Task<ActionResult> ExportWareById(int id)
    {
        var data = await _dataService.ExportWareByIdToJsonAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        return File(data, "application/json", $"ware_{id}.json");
    }
}