namespace Avans.FoodWaste.Core.Dtos;

public class ErrorResponseDto
{
    public string Message { get; set; }
    public string? Details { get; set; } // Optional for additional details
}