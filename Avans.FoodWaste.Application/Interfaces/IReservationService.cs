using Avans.FoodWaste.Core.Dtos;
using Avans.FoodWaste.Core.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avans.FoodWaste.Application.Interfaces
{
    public interface IReservationService
    {
        Task<Result<ReservationDto>> CreateAsync(CreateReservationDto dto);
        Task<Result<ReservationDto>> GetByIdAsync(int id);       // Get a specific reservation
        Task<Result<IEnumerable<ReservationDto>>> GetAllAsync(); // Get all reservations
        Task<Result<IEnumerable<ReservationDto>>> GetByStudentIdAsync(int studentId); // Get reservations by student
        Task<Result<bool>> DeleteAsync(int id);
        
        Task<Result<IEnumerable<ReservationDetailsDto>>> GetReservationsWithDetailsByStudentIdAsync(int studentId);
    }
}