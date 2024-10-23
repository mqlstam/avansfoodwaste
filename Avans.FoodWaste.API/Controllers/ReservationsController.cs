using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avans.FoodWaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }
        [HttpPost]
        [Authorize] // Protect the Create action
        public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto dto)
        {
            var result = await _reservationService.CreateAsync(dto);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Create), new { id = result.Value.Id }, result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        [Authorize] // Protect the GetById action
        public async Task<ActionResult<ReservationDto>> GetById(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet]
        [Authorize] //Protect the GetAll action
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
        {
            var result = await _reservationService.GetAllAsync();
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet("student/{studentId}")]
        [Authorize] // Protect the GetByStudentId action
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetByStudentId(int studentId)
        {
            var result = await _reservationService.GetByStudentIdAsync(studentId);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet("student/{studentId}/details")]
        [Authorize] // Protect the GetReservationsWithDetailsByStudentId action
        public async Task<ActionResult<IEnumerable<ReservationDetailsDto>>> GetReservationsWithDetailsByStudentId(int studentId)
        {
            var result = await _reservationService.GetReservationsWithDetailsByStudentIdAsync(studentId);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "CafetariaStaff")] // Only CafeteriaStaff can delete reservations
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservationService.DeleteAsync(id);
            return result.IsSuccess
                ? NoContent()
                : BadRequest(result.Error);
        }
    }
}