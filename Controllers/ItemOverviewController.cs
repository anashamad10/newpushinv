using Inventory.Models.Dto_s.Item;
using Inventory.Models.Models;
using Inventory.Repository.Repository.ItemOverviewRepo;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventotryProjectPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemOverviewController : ControllerBase
    {
        private IItemOverviewRepository _itemOverviewRepository;
        public ItemOverviewController(IItemOverviewRepository itemOverviewRepository)
        {
            _itemOverviewRepository = itemOverviewRepository;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int pageSize = 10, int pageNumber = 1)
        {
            var itemsOverview = await _itemOverviewRepository.GetAll(pageNumber, pageSize);
            return Ok(itemsOverview);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ItemOverview dto)
        {
            if (ModelState.IsValid)
            {
                await _itemOverviewRepository.AddAsync(dto);
                return Ok("created!!!");
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(ItemOverview dto)
        {
            if (ModelState.IsValid)
            {
                await _itemOverviewRepository.UpdateAsync(dto);
                return Ok("Successs");
            }
            return BadRequest(ModelState);
        }
        [HttpGet("Delete/id")]
        public async Task<IActionResult> Delete(int id)
        {
            await _itemOverviewRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}

