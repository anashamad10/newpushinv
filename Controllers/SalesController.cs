using Inventory.Models.Dto_s;
using Inventory.Models.Dto_s.Sales;
using Inventory.Repository.Repository.SalesRepo;
using Inventory.ViewModel.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventotryProjectPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class SalesController : Controller
    {
        private ISalesRepository _salesRepository;
        public SalesController(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }
        [HttpGet("Indexr")]
        public async Task<IActionResult> Index(int pageSize = 10, int pageNumber = 1)
        {
            var salesTypes = await _salesRepository.GetAll(pageNumber, pageSize); // Use GetAllAsync
            return Ok(salesTypes); // Return a proper HTTP response, like Ok
        }
        [HttpGet("GetSaleOrderandInvoice")]
        public async Task<IActionResult> GetSalesOrder()
        {
            var salesOrder = await _salesRepository.GetSalesCode(); // Call GetAll without any parameters
            return Ok(salesOrder); // Return a proper HTTP response, like Ok
        }

        [HttpGet("GetSalesOrderID")]
        public async Task<IActionResult> GetBySalesId(int id)
        {
            var salesOrder = await _salesRepository.GetBySalesIdAsync(id);
            if(salesOrder == null)
            {
                return NotFound();
            }
            return Ok(salesOrder); // Return a proper HTTP response, like Ok
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(SalesListViewDto dto) // Make the method asynchronous
        {
            if (ModelState.IsValid)
            {
                await _salesRepository.AddAsync(dto); // Use AddAsync
                return Ok(new { status = "success", message = "Item created successfully" });
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        [HttpGet("api/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _salesRepository.GetByIdAsync(id);

            if (dto == null)
            {
                return NotFound(); // Return a 404 response if the item is not found
            }

            return Ok(dto); // Return a JSON response with the data
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SalesListViewDto dto) // Make the method asynchronous
        {
            if (ModelState.IsValid)
            {
                await _salesRepository.UpdateAsync(dto); // Use UpdateAsync
                return Ok(new { status = "success", message = "Item created successfully" });
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id) // Make the method asynchronous
        {
            await _salesRepository.DeleteAsync(id); // Use DeleteAsync
            return Ok(new { status = "success", message = "Item created successfully" });
        }

    }
}
