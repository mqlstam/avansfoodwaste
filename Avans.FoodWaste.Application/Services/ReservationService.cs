using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Core.Dtos;
using Avans.FoodWaste.Core.Entities;
using Avans.FoodWaste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Avans.FoodWaste.Core.Results;
using Avans.FoodWaste.Core.Helpers;
using System.Linq;

namespace Avans.FoodWaste.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly FoodWasteDbContext _context;
        private readonly IPackageService _packageService;

        public ReservationService(FoodWasteDbContext context, IPackageService packageService)
        {
            _context = context;
            _packageService = packageService;
        }

        private ReservationDto MapToDto(Reservation reservation)
        {
            if (reservation == null) return null;

            return new ReservationDto
            {
                Id = reservation.Id,
                StudentId = reservation.StudentId,
                PackageId = reservation.PackageId,
                ReservationDate = reservation.ReservationDate
            };
        }

        public async Task<Result<ReservationDto>> CreateAsync(CreateReservationDto dto)
        {
            try
            {
                // 1. Check if student and package exist
                var student = await _context.Students.FindAsync(dto.StudentId);
                if (student == null)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {dto.StudentId} not found." }
                    };
                }

                var package = await _context.Packages.FindAsync(dto.PackageId);
                if (package == null)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Package not found.", Details = $"Package with ID {dto.PackageId} not found." }
                    };
                }

                // 2. Check if package is already reserved
                if (package.ReservationStatus != ReservationStatus.Available)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Package already reserved.", Details = $"Package with ID {dto.PackageId} is already reserved." }
                    };
                }

                // 3. Check if student already has a reservation for this day
                if (await _context.Reservations.AnyAsync(r =>
                        r.StudentId == dto.StudentId &&
                        r.Package.PickupDateTime.Date == package.PickupDateTime.Date))
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student already has a reservation for this day.", Details = $"Student with ID {dto.StudentId} already has a reservation for {package.PickupDateTime.Date}." }
                    };
                }

                // 4. Check for age restriction
                if (package.IsAdultPackage && DateHelpers.CalculateAge(student.DateOfBirth) < 18) // Corrected call
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Age restriction.", Details = "Student is not old enough to reserve this package." }
                    };
                }


                // 5. Create and save reservation
                var reservation = new Reservation
                {
                    StudentId = dto.StudentId,
                    PackageId = dto.PackageId
                };

                _context.Reservations.Add(reservation);
                package.ReservationStatus = ReservationStatus.Reserved;
                package.ReservedById = student.Id;
                await _context.SaveChangesAsync();

                return new Result<ReservationDto> { IsSuccess = true, Value = MapToDto(reservation) };
            }
            catch (Exception ex)
            {
                return new Result<ReservationDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while creating the reservation.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<ReservationDto>> GetByIdAsync(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Student)
                    .Include(r => r.Package)
                    .ThenInclude(p => p.Cafeteria)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Reservation not found.", Details = $"Reservation with ID {id} not found." }
                    };
                }

                return new Result<ReservationDto> { IsSuccess = true, Value = MapToDto(reservation) };
            }
            catch (Exception ex)
            {
                return new Result<ReservationDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while retrieving the reservation.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetAllAsync()
        {
            try
            {
                var reservations = await _context.Reservations
                    .Include(r => r.Student)
                    .Include(r => r.Package)
                    .ThenInclude(p => p.Cafeteria)
                    .ToListAsync();

                return new Result<IEnumerable<ReservationDto>> { IsSuccess = true, Value = reservations.Select(MapToDto) };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<ReservationDto>>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "Failed to retrieve all reservations.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<IEnumerable<ReservationDto>>> GetByStudentIdAsync(int studentId)
        {
            try
            {
                // 1. Check if student exists
                var student = await _context.Students.FindAsync(studentId);
                if (student == null)
                {
                    return new Result<IEnumerable<ReservationDto>>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {studentId} not found." }
                    };
                }

                // 2. If student exists, retrieve reservations
                var reservations = await _context.Reservations
                    .Include(r => r.Student)
                    .Include(r => r.Package)
                    .ThenInclude(p => p.Cafeteria)
                    .Where(r => r.StudentId == studentId)
                    .ToListAsync();

                return new Result<IEnumerable<ReservationDto>> { IsSuccess = true, Value = reservations.Select(MapToDto) };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<ReservationDto>>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "Failed to retrieve reservations.", Details = ex.Message }
                };
            }
        }

        // In Application/Services/ReservationService.cs
        public async Task<Result<IEnumerable<ReservationDetailsDto>>> GetReservationsWithDetailsByStudentIdAsync(int studentId)
        {
            try
            {
                // 1. Check if student exists
                var student = await _context.Students.FindAsync(studentId);
                if (student == null)
                {
                    return new Result<IEnumerable<ReservationDetailsDto>>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {studentId} not found." }
                    };
                }

                // 2. If student exists, retrieve reservations with details
                var reservations = await _context.Reservations
                    .Include(r => r.Student)
                    .Include(r => r.Package)
                    .ThenInclude(p => p.Cafeteria)
                    .Where(r => r.StudentId == studentId)
                    .ToListAsync();

                return new Result<IEnumerable<ReservationDetailsDto>>
                {
                    IsSuccess = true,
                    Value = reservations.Select(MapToReservationDetailsDto).ToList() 
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<ReservationDetailsDto>>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "Failed to retrieve reservation details.", Details = ex.Message }
                };
            }
        }

        private ReservationDetailsDto MapToReservationDetailsDto(Reservation reservation)
        {
            return new ReservationDetailsDto
            {
                ReservationId = reservation.Id,
                StudentId = reservation.StudentId,
                ReservationDate = reservation.ReservationDate,

                PackageId = reservation.Package.Id,
                PackageName = reservation.Package.Name,
                ExampleProductIds = reservation.Package.ExampleProductIds,
                PickupDateTime = reservation.Package.PickupDateTime,
                LatestPickupTime = reservation.Package.LatestPickupTime,
                IsAdultPackage = reservation.Package.IsAdultPackage,
                Price = reservation.Package.Price,
                MealType = reservation.Package.MealType.ToString(),
                ReservationStatus = reservation.Package.ReservationStatus.ToString(),
                NoShowStatus = reservation.Package.NoShowStatus.ToString(),

                Cafeteria = new CafeteriaDto
                {
                    Id = reservation.Package.Cafeteria.Id,
                    City = reservation.Package.Cafeteria.City,
                    LocationIdentifier = reservation.Package.Cafeteria.LocationIdentifier,
                    HotMealsAvailable = reservation.Package.Cafeteria.HotMealsAvailable,
                    OperatingHours = reservation.Package.Cafeteria.OperatingHours
                }
            };
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Reservation not found.", Details = $"Reservation with ID {id} not found." }
                    };
                }

                var package = await _context.Packages.FirstOrDefaultAsync(x => x.Id == reservation.PackageId);
                if (package != null)
                {
                    package.ReservationStatus = ReservationStatus.Available;
                    package.ReservedById = null;
                }


                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
                return new Result<bool> { IsSuccess = true, Value = true };

            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred deleting the reservation", Details = ex.Message }
                };
            }
        }
    }
}