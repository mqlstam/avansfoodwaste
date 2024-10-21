using System;
using System.ComponentModel.DataAnnotations;

namespace Avans.FoodWaste.Core.Dtos
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; } // Age validation is in service

        [Required(ErrorMessage = "Student number is required.")]
        [StringLength(20, ErrorMessage = "Student number cannot exceed 20 characters.")]
        public string StudentNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "Study city cannot exceed 50 characters.")]
        public string StudyCity { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
    }
}