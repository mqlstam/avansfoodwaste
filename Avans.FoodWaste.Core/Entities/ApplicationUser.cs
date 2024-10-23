using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string Role { get; set; } // Student or Staff
    // Link to either Student or CafeteriaStaff entity
    public int? StudentId { get; set; }
    public int? CafeteriaStaffId { get; set; }
}