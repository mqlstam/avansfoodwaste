namespace Avans.FoodWaste.Core.Exceptions;

public class HotMealsNotAvailableException : Exception
{
    public HotMealsNotAvailableException(int cafeteriaId) 
        : base($"Cafeteria with ID {cafeteriaId} does not support hot meals.") { }
}