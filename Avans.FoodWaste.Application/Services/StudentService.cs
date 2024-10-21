using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Core.Dtos;
using Avans.FoodWaste.Core.Entities;
using Avans.FoodWaste.Core.Helpers;
using Avans.FoodWaste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Avans.FoodWaste.Core.Results;

namespace Avans.FoodWaste.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly FoodWasteDbContext _context;

        public StudentService(FoodWasteDbContext context)
        {
            _context = context;
        }

        private StudentDto MapToDto(Student student)
        {
            if (student == null) return null;

            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                DateOfBirth = student.DateOfBirth,
                StudentNumber = student.StudentNumber,
                Email = student.Email,
                StudyCity = student.StudyCity,
                PhoneNumber = student.PhoneNumber,
                NoShowCounter = student.NoShowCounter
            };
        }

        public async Task<Result<IEnumerable<StudentDto>>> GetAllAsync()
        {
            try
            {
                var students = await _context.Students.ToListAsync();
                return new Result<IEnumerable<StudentDto>> { IsSuccess = true, Value = students.Select(MapToDto) };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<StudentDto>>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while retrieving students.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<StudentDto>> GetByIdAsync(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);

                if (student == null)
                {
                    return new Result<StudentDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {id} not found." }
                    };
                }

                return new Result<StudentDto> { IsSuccess = true, Value = MapToDto(student) };
            }
            catch (Exception ex)
            {
                return new Result<StudentDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while retrieving the student.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto)
        {
            try
            {
                // 1. Validate Age (must be at least 16)
                int age = DateHelpers.CalculateAge(dto.DateOfBirth);  // Use the helper function
                if (age < 16)
                {
                    return new Result<StudentDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Invalid date of birth.", Details = "Student must be at least 16 years old." }
                    };
                }
                // Check for duplicate student number
                if (await _context.Students.AnyAsync(s => s.StudentNumber == dto.StudentNumber))
                {
                    return new Result<StudentDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student number already exists.", Details = "A student with this student number already exists." }
                    };
                }

                // 2. Create Student entity
                var student = new Student
                {
                    Name = dto.Name,
                    DateOfBirth = dto.DateOfBirth,
                    StudentNumber = dto.StudentNumber,
                    Email = dto.Email,
                    StudyCity = dto.StudyCity,
                    PhoneNumber = dto.PhoneNumber,
                    NoShowCounter = 0 // Initialize NoShowCounter to 0
                };
                



                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return new Result<StudentDto> { IsSuccess = true, Value = MapToDto(student) };

            }
            catch (Exception ex)
            {
                // Handle other potential errors (e.g., duplicate student numbers, database errors)
                return new Result<StudentDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while creating the student.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<StudentDto>> UpdateAsync(int id, UpdateStudentDto dto)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return new Result<StudentDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {id} not found." }
                    };
                }

                // 1. Validate Age (if DateOfBirth is being updated)
                if (dto.DateOfBirth != student.DateOfBirth)
                {
                    int age = DateHelpers.CalculateAge(dto.DateOfBirth); // Use the helper function
                    if (age < 16)
                    {
                        return new Result<StudentDto>
                        {
                            IsSuccess = false,
                            Error = new ErrorResponseDto { Message = "Invalid date of birth.", Details = "Student must be at least 16 years old." }
                        };
                    }
                }
                
                // Check for duplicate student number (if it's changed)
                if (dto.StudentNumber != student.StudentNumber && await _context.Students.AnyAsync(s => s.StudentNumber == dto.StudentNumber))
                {
                    return new Result<StudentDto>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student number already exists.", Details = "A student with this student number already exists." }
                    };
                }

                // 2. Update Student entity
                student.Name = dto.Name;
                student.DateOfBirth = dto.DateOfBirth;
                student.StudentNumber = dto.StudentNumber;
                student.Email = dto.Email;
                student.StudyCity = dto.StudyCity;
                student.PhoneNumber = dto.PhoneNumber; 

                await _context.SaveChangesAsync();
                return new Result<StudentDto> { IsSuccess = true, Value = MapToDto(student) };
            }
            catch (Exception ex)
            {
                // Handle other potential errors
                return new Result<StudentDto>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while updating the student.", Details = ex.Message }
                };
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return new Result<bool>
                    {
                        IsSuccess = false,
                        Error = new ErrorResponseDto { Message = "Student not found.", Details = $"Student with ID {id} not found." }
                    };
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return new Result<bool> { IsSuccess = true, Value = true };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorResponseDto { Message = "An error occurred while deleting the student.", Details = ex.Message }
                };
            }
        }
    }
}