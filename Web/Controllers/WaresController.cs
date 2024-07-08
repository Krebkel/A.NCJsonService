using Models;
using Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaresController : ControllerBase
{
    private readonly WareRepository _wareRepository;

    public WaresController(WareRepository wareRepository)
    {
        _wareRepository = wareRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ware>>> GetWares()
    {
        var wares = await _wareRepository.GetWaresAsync();
        return Ok(wares);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ware>> GetWare(int id)
    {
        var ware = await _wareRepository.GetWareByIdAsync(id);
        if (ware == null)
        {
            return NotFound();
        }
        return Ok(ware);
    }

    [HttpPost]
    public async Task<ActionResult> CreateWare(Ware ware)
    {
        await _wareRepository.CreateWareAsync(ware);
        return CreatedAtAction(nameof(GetWare), new { id = ware.Id }, ware);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateWare(int id, Ware ware)
    {
        if (id != ware.Id)
        {
            return BadRequest();
        }

        await _wareRepository.UpdateWareAsync(ware);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWare(int id)
    {
        await _wareRepository.DeleteWareAsync(id);
        return NoContent();
    }
}