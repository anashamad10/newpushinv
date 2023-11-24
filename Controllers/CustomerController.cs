using Inventory.Models.Models;
using Inventory.Models.Dto_s;
using Inventory.Repository.Repository.CustomerRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inventory.ViewModel.Customer;

namespace InventotryProjectPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class CustomerController : ControllerBase
    {
        private ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int pageSize = 10, int pageNumber = 1)
        {
            var customers = await _customerRepository.GetAll(pageNumber, pageSize); // Use GetAllAsync
            return Ok(customers); // Return a proper HTTP response, like Ok
        }
        [HttpGet("customers")]
        public async Task<IActionResult> GetCompany()
        {
            var customers = await _customerRepository.GetCompanyAsync(); // Call GetAll without any parameters
            return Ok(customers); // Return a proper HTTP response, like Ok
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(CustomerListViewDto dto) // Make the method asynchronous
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.AddAsync(dto); // Use AddAsync
                return Ok(new { status = "success", message = "Item created successfully" });
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        [HttpGet("api/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _customerRepository.GetByIdAsync(id);

            if (dto == null)
            {
                return NotFound(); // Return a 404 response if the item is not found
            }

            return Ok(dto); // Return a JSON response with the data
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(CustomerListViewDto dto) // Make the method asynchronous
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.UpdateAsync(dto); // Use UpdateAsync
                return Ok("successfully created");
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id) // Make the method asynchronous
        {
            await _customerRepository.DeleteAsync(id); // Use DeleteAsync
            return RedirectToAction("Index");
        }
    }
}

