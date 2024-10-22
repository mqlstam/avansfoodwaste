using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Core.Dtos;
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
        public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto dto)
        {
            var result = await _reservationService.CreateAsync(dto);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Create), new { id = result.Value.Id }, result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetById(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
        {
            var result = await _reservationService.GetAllAsync();
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }


        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetByStudentId(int studentId)
        {
            var result = await _reservationService.GetByStudentIdAsync(studentId);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet("student/{studentId}/details")] // New route
        public async Task<ActionResult<IEnumerable<ReservationDetailsDto>>> GetReservationsWithDetailsByStudentId(int studentId)
        {
            var result = await _reservationService.GetReservationsWithDetailsByStudentIdAsync(studentId);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservationService.DeleteAsync(id);
            return result.IsSuccess
                ? NoContent()
                : BadRequest(result.Error);
        }
    }
}