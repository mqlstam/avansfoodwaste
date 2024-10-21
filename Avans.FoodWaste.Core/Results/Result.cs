using Avans.FoodWaste.Core.Dtos;

namespace Avans.FoodWaste.Core.Results;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; } 
    public ErrorResponseDto? Error { get; set; }
}