using Models;
using Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly ItemRepository _itemRepository;

    public ItemController(ItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        var items = await _itemRepository.GetItemsAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetItem(int id)
    {
        var item = await _itemRepository.GetItemByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult> CreateItem(Item item)
    {
        await _itemRepository.CreateItemAsync(item);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateItem(int id, Item item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }

        await _itemRepository.UpdateItemAsync(item);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItem(int id)
    {
        await _itemRepository.DeleteItemAsync(id);
        return NoContent();
    }
}