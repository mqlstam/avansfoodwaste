namespace Avans.FoodWaste.Core.Exceptions;

public class StudentHasExistingReservationException : Exception
{
    public StudentHasExistingReservationException(int studentId) 
        : base($"Student with ID {studentId} already has a reservation for this day.") { }
}