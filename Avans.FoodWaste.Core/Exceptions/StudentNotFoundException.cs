namespace Avans.FoodWaste.Core.Exceptions;

public class StudentNotFoundException : Exception
{
    public StudentNotFoundException(int studentId)
        : base($"Student with ID {studentId} not found.") { }
}