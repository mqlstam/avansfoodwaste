using System;
using System.Collections.Generic;

namespace Avans.FoodWaste.Core.Dtos
{
    public class PackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> ExampleProductIds { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime LatestPickupTime { get; set; }
        public bool IsAdultPackage { get; set; }
        public decimal Price { get; set; }
        public string MealType { get; set; } // Changed to String
        public int? ReservedById { get; set; }
        public string ReservationStatus { get; set; } // Changed to String
        public string NoShowStatus { get; set; } // Changed to String
        public int CafeteriaId { get; set; }
        public CafeteriaDto Cafeteria { get; set; } // CafeteriaDto
    }


    public class CafeteriaDto
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string LocationIdentifier { get; set; }
        public bool HotMealsAvailable { get; set; }
        public string OperatingHours { get; set; }

    }
}