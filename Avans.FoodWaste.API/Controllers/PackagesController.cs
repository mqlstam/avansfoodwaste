using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Core.Dtos; // For the IPackageService we'll create next
using Avans.FoodWaste.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avans.FoodWaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authentication for all actions in this controller
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService; // Dependency injection

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageDto>>> GetAll()
        {
            var result = await _packageService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PackageDto>> GetById(int id)
        {
            var result = await _packageService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpPost] 
        public async Task<ActionResult<PackageDto>> Create([FromBody] CreatePackageDto createPackageDto)
        {
            var result = await _packageService.CreateAsync(createPackageDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpPut("{id}")] 
        public async Task<ActionResult<PackageDto>> Update(int id, [FromBody] UpdatePackageDto updatePackageDto)
        {
            var result = await _packageService.UpdateAsync(id, updatePackageDto);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _packageService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
        
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<PackageDto>>> GetAvailable(string? city = null, MealType? mealType = null, string? orderBy = null) 
        {
            var result = await _packageService.GetAvailablePackagesAsync(city, mealType, orderBy);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }


        [HttpGet("overview/{studentId}")] // New endpoint for student package overview
        public async Task<ActionResult<StudentPackageOverviewDto>> GetPackageOverview(int studentId)
        {
            var result = await _packageService.GetPackageOverviewAsync(studentId);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}