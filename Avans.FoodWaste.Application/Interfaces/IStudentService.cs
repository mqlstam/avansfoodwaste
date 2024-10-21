using Avans.FoodWaste.Core.Dtos;
using Avans.FoodWaste.Core.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avans.FoodWaste.Application.Interfaces
{
    public interface IStudentService
    {
        Task<Result<IEnumerable<StudentDto>>> GetAllAsync();
        Task<Result<StudentDto>> GetByIdAsync(int id);
        Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto);
        Task<Result<StudentDto>> UpdateAsync(int id, UpdateStudentDto dto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}