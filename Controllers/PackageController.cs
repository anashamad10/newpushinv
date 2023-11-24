using Inventory.Models.Dto_s.Package;
using Inventory.Repository.Repository.PackageRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventotryProjectPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PackageController : ControllerBase
    {
        private IPackageRepository _packageRepository;
        public PackageController(IPackageRepository packageRepository) 
        {
            _packageRepository = packageRepository;
        }
        // GET: api/<PackageController>
        [HttpGet("Indexr")]
        public async Task<IActionResult> Index(int pageSize = 10, int pageNumber = 1)
        {
           var packageTypes = await _packageRepository.GetAll(pageNumber, pageSize);
            return Ok(packageTypes);
        }

        // POST api/<PackageController>
        [HttpPost("create")]
        public async Task<IActionResult> Create(PackageListDto dto)
        {
            if (ModelState.IsValid)
            {
                await _packageRepository.AddAsync(dto);
                return Ok(new { status = "success", message = "package created successfully" });
            }
            return BadRequest(ModelState); // Return BadRequest if the model state is not valid
        }

        // PUT api/<PackageController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PackageController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
