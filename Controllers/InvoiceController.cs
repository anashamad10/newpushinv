using Inventory.Models.Dto_s;
using Inventory.Models.Dto_s.Invoice;
using Inventory.Repository.Repository.InvoiceTypeRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventotryProjectPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class InvoiceController : Controller
    {
        private IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(int pageSize = 10, int pageNumber = 1)
        {
            var invoices = await _invoiceRepository.GetAll(pageNumber, pageSize); // Use GetAllAsync
            return Ok(invoices); // Return a proper HTTP response, like Ok
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet("GetRef")]
        public async Task<IActionResult> GetInvoice()
        {
            var invoice = await _invoiceRepository.GetInvoiceAsync(); // Call GetAll without any parameters
            return Ok(invoice); // Return a proper HTTP response, like Ok
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(InvoiceListViewDto dto) // Make the method asynchronous
        {
            if (ModelState.IsValid)
            {
                await _invoiceRepository.AddAsync(dto); // Use AddAsync
                return Ok(new { status = "success", message = "Item created successfully" });
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        [HttpGet("api/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _invoiceRepository.GetByIdAsync(id);

            if (dto == null)
            {
                return NotFound(); // Return a 404 response if the item is not found
            }

            return Ok(dto); // Return a JSON response with the data
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(InvoiceListViewDto dto) // Make the method asynchronous
        {
            if (ModelState.IsValid)
            {
                await _invoiceRepository.UpdateAsync(dto); // Use UpdateAsync
                return Ok("success");
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id) // Make the method asynchronous
        {
            await _invoiceRepository.DeleteAsync(id); // Use DeleteAsync
            return RedirectToAction("Index");
        }
    }
}
