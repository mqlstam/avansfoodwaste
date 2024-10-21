namespace Avans.FoodWaste.Core.Helpers;

public static class DateHelpers
{
    public static int CalculateAge(DateTime dateOfBirth)
    {
        int age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) 
        {
            age--;
        }
        return age;
    }
}