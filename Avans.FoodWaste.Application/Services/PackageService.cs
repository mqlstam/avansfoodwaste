using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Core.Entities;
using Avans.FoodWaste.Infrastructure.Data; // For data access
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Avans.FoodWaste.Core.Dtos;
using Avans.FoodWaste.Core.Exceptions;
using Avans.FoodWaste.Core.Results;


namespace Avans.FoodWaste.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly FoodWasteDbContext _context;

        public PackageService(FoodWasteDbContext context)
        {
            _context = context;
        }

        private PackageDto MapToDto(Package package)
        {
            if (package == null) return null;

            return new PackageDto
            {
                Id = package.Id,
                Name = package.Name,
                ExampleProductIds = package.ExampleProductIds, // Map this as well
                PickupDateTime = package.PickupDateTime, // Map the PickupDateTime
                LatestPickupTime = package.LatestPickupTime,
                IsAdultPackage = package.IsAdultPackage,
                Price = package.Price,
                MealType = package.MealType.ToString(),
                ReservedById = package.ReservedById,
                ReservationStatus = package.ReservationStatus.ToString(),
                NoShowStatus = package.NoShowStatus.ToString(),
                CafeteriaId = package.CafeteriaId, // You might not need this as you have the whole Cafeteria object
                Cafeteria = new CafeteriaDto // Map Cafeteria data
                {
                    Id = package.Cafeteria.Id,
                    City = package.Cafeteria.City,
                    LocationIdentifier = package.Cafeteria.LocationIdentifier,
                    HotMealsAvailable = package.Cafeteria.HotMealsAvailable,
                    OperatingHours = package.Cafeteria.OperatingHours
                }
            };
        }

        public async Task<Result<IEnumerable<PackageDto>>> GetAllAsync()
        {
            try
            {
                var packages = await _context.Packages
                    .Include(p => p.Cafeteria)
                    .ToListAsync();

                var packageDtos = packages.Select(MapToDto);
                return new Result<IEnumerable<PackageDto>> { IsSuccess = true, Value = packageDtos };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<PackageDto>>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while retrieving packages.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<PackageDto>> GetByIdAsync(int id)
        {
            try
            {
                var package = await _context.Packages
                    .Include(p => p.Cafeteria)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (package == null)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Package not found.", Details = $"Package with ID {id} not found." }
                    };
                }

                return new Result<PackageDto> { IsSuccess = true, Value = MapToDto(package) };
            }
            catch (Exception ex)
            {
                return new Result<PackageDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while retrieving the package.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<PackageDto>> CreateAsync(CreatePackageDto dto)
        {
            try
            {
                // 1. Validate Cafeteria
                var cafeteria = await _context.Cafeterias.FindAsync(dto.CafeteriaId);
                if (cafeteria == null)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Cafeteria not found.", Details = $"Cafeteria with ID {dto.CafeteriaId} not found." }
                    };
                }

                // 2. Validate if Cafeteria allows ANY packages (hot meals or not)
                if (!cafeteria.HotMealsAvailable)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Hot meals are not available at the specified cafeteria.", Details = $"Cafeteria with ID {dto.CafeteriaId} does not support hot meals." }
                    };
                }

                // 3. Validate Product IDs
                List<int> invalidProductIds = new List<int>();
                foreach (var productId in dto.ExampleProductIds)
                {
                    if (!await _context.Products.AnyAsync(p => p.Id == productId))
                    {
                        invalidProductIds.Add(productId);
                    }
                }

                if (invalidProductIds.Count > 0)
                {
                    string errorMessage = $"Invalid product IDs: {string.Join(", ", invalidProductIds)}";
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "One or more of the specified products were not found.", Details = errorMessage }
                    };
                }

                // 4. Validate DateTimes
                if (dto.PickupDateTime < DateTime.Now)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Pickup date and time cannot be in the past.", Details = "Pickup date and time cannot be in the past." }
                    };
                }

                if (dto.LatestPickupTime <= dto.PickupDateTime)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Latest pickup time must be after the pickup date and time.", Details = "Latest pickup time must be after the pickup date and time." }
                    };
                }

                if (dto.PickupDateTime > DateTime.Now.AddDays(2))
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Packages can be planned a maximum of 2 days in advance.", Details = "Packages can be planned a maximum of 2 days in advance." }
                    };
                }

                // 5. Check for alcohol and set IsAdultPackage
                bool containsAlcohol = await _context.Products
                    .Where(x => dto.ExampleProductIds.Contains(x.Id))
                    .AnyAsync(p => p.ContainsAlcohol);

                // 6. Create the Package entity
                var package = new Package
                {
                    Name = dto.Name,
                    ExampleProductIds = dto.ExampleProductIds,
                    PickupDateTime = dto.PickupDateTime,
                    LatestPickupTime = dto.LatestPickupTime,
                    Price = dto.Price,
                    MealType = dto.MealType,
                    CafeteriaId = dto.CafeteriaId,
                    IsAdultPackage = containsAlcohol,
                    ReservationStatus = ReservationStatus.Available,
                    NoShowStatus = NoShowStatus.None
                };

                _context.Packages.Add(package);
                await _context.SaveChangesAsync();

                return new Result<PackageDto> { IsSuccess = true, Value = MapToDto(package) };
            }
            catch (Exception ex)
            {
                return new Result<PackageDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while creating the package.", Details = ex.Message }
                };
            }
        }
        public async Task<Result<PackageDto>> UpdateAsync(int id, UpdatePackageDto dto)
        {
            try
            {
                var package = await _context.Packages.FindAsync(id);
                if (package == null)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Package not found.", Details = $"Package with ID {id} not found." }
                    };
                }

                // 1. Validate Cafeteria (If CafeteriaId is being updated)
                if (dto.CafeteriaId != package.CafeteriaId) // Only check if it's changed
                {
                    var cafeteria = await _context.Cafeterias.FindAsync(dto.CafeteriaId);
                    if (cafeteria == null)
                    {
                        return new Result<PackageDto>
                        {
                            IsSuccess = false,
                            Error = new ErrorResponseDto { Message = "The specified cafeteria was not found.", Details = $"Cafeteria with ID {dto.CafeteriaId} not found." }
                        };
                    }

                    if (!cafeteria.HotMealsAvailable)
                    {
                        return new Result<PackageDto>
                        {
                            IsSuccess = false,
                            Error = new ErrorResponseDto { Message = "Hot meals are not available at the specified cafeteria.", Details = $"Cafeteria with ID {dto.CafeteriaId} does not support hot meals." }
                        };
                    }
                }


                // 2. Validate Product IDs (If ExampleProductIds are being updated)
                if (!Enumerable.SequenceEqual(dto.ExampleProductIds, package.ExampleProductIds))
                {
                    List<int> invalidProductIds = new List<int>();
                    foreach (var productId in dto.ExampleProductIds)
                    {
                        if (!await _context.Products.AnyAsync(p => p.Id == productId))
                        {
                            invalidProductIds.Add(productId);
                        }
                    }

                    if (invalidProductIds.Count > 0)
                    {
                        string errorMessage = $"Invalid product IDs: {string.Join(", ", invalidProductIds)}";
                        return new Result<PackageDto>
                        {
                            IsSuccess = false,
                            Error = new ErrorResponseDto { Message = "One or more of the specified products were not found.", Details = errorMessage }
                        };
                    }
                }


                // 3. Validate DateTimes
                if (dto.PickupDateTime < DateTime.Now)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Invalid date or time provided. Please check your input.", Details = "Pickup date and time cannot be in the past." }
                    };
                }

                if (dto.LatestPickupTime <= dto.PickupDateTime)
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Invalid date or time provided. Please check your input.", Details = "Latest pickup time must be after the pickup date and time." }
                    };
                }

                if (dto.PickupDateTime > DateTime.Now.AddDays(2))
                {
                    return new Result<PackageDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Invalid date or time provided. Please check your input.", Details = "Packages can be planned a maximum of 2 days in advance." }
                    };
                }


                // 4. Check for alcohol and set IsAdultPackage
                // Note: This is re-evaluated even if ProductIds haven't changed
                bool containsAlcohol = await _context.Products
                    .Where(x => dto.ExampleProductIds.Contains(x.Id))
                    .AnyAsync(p => p.ContainsAlcohol);


                // Update the package entity
                package.Name = dto.Name;
                package.ExampleProductIds = dto.ExampleProductIds;
                package.PickupDateTime = dto.PickupDateTime;
                package.LatestPickupTime = dto.LatestPickupTime;
                package.Price = dto.Price;
                package.MealType = dto.MealType;
                package.CafeteriaId = dto.CafeteriaId;
                package.IsAdultPackage = containsAlcohol; // Always update, in case products changed

                await _context.SaveChangesAsync();
                return new Result<PackageDto> { IsSuccess = true, Value = MapToDto(package) };
            }
            catch (Exception ex)
            {
                return new Result<PackageDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while updating the package.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var package = await _context.Packages.FindAsync(id);
                if (package == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Package not found.", Details = $"Package with ID {id} not found." }
                    };
                }
                //Check if the package has already been reserved. We are not allowed to delete reserved packages.
                if (package.ReservationStatus == ReservationStatus.Reserved)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Cannot delete package.", Details = "Cannot delete a reserved package" }
                    };
                }

                _context.Packages.Remove(package);
                await _context.SaveChangesAsync();
                return new Result<bool> { IsSuccess = true, Value = true }; 
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while deleting the package.", Details = ex.Message }
                };
            }
        }
    }
}