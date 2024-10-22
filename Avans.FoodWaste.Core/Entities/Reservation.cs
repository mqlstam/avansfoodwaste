using System;

namespace Avans.FoodWaste.Core.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }  // Navigation property
        public int PackageId { get; set; }
        public Package Package { get; set; }  // Navigation property
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow; //Automatically sets time of reservation.
        
        

    }
}