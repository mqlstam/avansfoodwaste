// In Application/Interfaces/IPackageService.cs
using Avans.FoodWaste.Core.Dtos;
using Avans.FoodWaste.Core.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avans.FoodWaste.Application.Interfaces
{
    public interface IPackageService
    {
        Task<Result<IEnumerable<PackageDto>>> GetAllAsync();
        Task<Result<PackageDto>> GetByIdAsync(int id);
        Task<Result<PackageDto>> CreateAsync(CreatePackageDto dto);
        Task<Result<PackageDto>> UpdateAsync(int id, UpdatePackageDto dto); 
        Task<Result<bool>> DeleteAsync(int id); 
    }
}
