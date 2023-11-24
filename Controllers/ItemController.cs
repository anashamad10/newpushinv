using Inventory.Models.Dto_s.Item;
using Inventory.Repository.Repository.ItemRepo;
using inventory_Models.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventotryProjectPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private IItemRepository _itemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int pageSize = 10, int pageNumber = 1)
        {
            var items = await _itemRepository.GetAll(pageNumber, pageSize);
            return Ok(items);
        }
        [HttpGet("item")]
        public async Task<IActionResult> GetBrand()
        {
            var item = await _itemRepository.GetItemBrandasync(); // Call GetAll without any parameters
            return Ok(item); // Return a proper HTTP response, like Ok
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(ItemListViewDto dto)
        {
            if (ModelState.IsValid)
            {
                await _itemRepository.AddAsync(dto);
                return Ok(new { status = "success", message = "Item created successfully" });
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(ItemListViewDto dto)
        {
            if (ModelState.IsValid)
            {
                await _itemRepository.UpdateAsync(dto);
                return Ok("Successs");
            }
            return BadRequest(ModelState);
        }
        [HttpGet("Delete/id")]
        public async Task<IActionResult> Delete(int id)
        {
            await _itemRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
