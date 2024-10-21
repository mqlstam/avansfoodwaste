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
                var student = await _context.Students.FindAsync(dto.StudentId);
                if (student == null)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {dto.StudentId} not found." }
                    };
                }

                var packageResult = await _packageService.GetByIdAsync(dto.PackageId);
                if (!packageResult.IsSuccess)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = packageResult.Error
                    };
                }

                var package = packageResult.Value;

                if (package.ReservationStatus != "Available")
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Package already reserved.", Details = $"Package with ID {dto.PackageId} is already reserved." }
                    };
                }

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

                if (package.IsAdultPackage && DateHelpers.CalculateAge(student.DateOfBirth) < 18)
                {
                    return new Result<ReservationDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Age restriction.", Details = "Student is not old enough to reserve this package." }
                    };
                }

                var reservation = new Reservation
                {
                    StudentId = dto.StudentId,
                    PackageId = dto.PackageId
                };

                _context.Reservations.Add(reservation);

                var packageEntity = await _context.Packages.FindAsync(dto.PackageId);

                if (packageEntity != null)
                {
                    packageEntity.ReservationStatus = ReservationStatus.Reserved;
                    packageEntity.ReservedById = student.Id;
                }

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
                var reservations = await _context.Reservations.ToListAsync();
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
                var reservations = await _context.Reservations
                    .Include(r => r.Student)
                    .Include(r => r.Package)
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