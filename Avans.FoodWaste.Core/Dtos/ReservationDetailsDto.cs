// In Core/Dtos/ReservationDetailsDto.cs
using System;
using System.Collections.Generic;

namespace Avans.FoodWaste.Core.Dtos
{
    public class ReservationDetailsDto
    {
        // Reservation Properties
        public int ReservationId { get; set; }
        public int StudentId { get; set; } 
        public DateTime ReservationDate { get; set; }

        // Package Properties
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public List<int> ExampleProductIds { get; set; } 
        public DateTime PickupDateTime { get; set; }
        public DateTime LatestPickupTime { get; set; }
        public bool IsAdultPackage { get; set; }
        public decimal Price { get; set; }
        public string MealType { get; set; }  
        public string ReservationStatus { get; set; }
        public string NoShowStatus { get; set; } 

        // Cafeteria Properties (Nested DTO)
        public CafeteriaDto Cafeteria { get; set; } 
    }
}