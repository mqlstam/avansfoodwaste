using System;

namespace Avans.FoodWaste.Core.Dtos
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int PackageId { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}