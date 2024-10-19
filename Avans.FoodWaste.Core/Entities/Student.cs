using System;

namespace Avans.FoodWaste.Core.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string StudentNumber { get; set; }
        public string Email { get; set; }
        public string StudyCity { get; set; }
        public string PhoneNumber { get; set; }
        public int NoShowCounter { get; set; }
    }
}