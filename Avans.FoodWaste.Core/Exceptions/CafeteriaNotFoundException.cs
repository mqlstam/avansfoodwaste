namespace Avans.FoodWaste.Core.Exceptions;

public class CafeteriaNotFoundException : Exception
{
    public CafeteriaNotFoundException(int cafeteriaId) 
        : base($"Cafeteria with ID {cafeteriaId} not found.") { }
}