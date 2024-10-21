using System;
using System.Collections.Generic;
using Avans.FoodWaste.Core.Entities;

namespace Avans.FoodWaste.Core.Dtos
{
    public class CreatePackageDto
    {
        public string Name { get; set; }
        public List<int> ExampleProductIds { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime LatestPickupTime { get; set; }
        public decimal Price { get; set; }
        public MealType MealType { get; set; }
        public int CafeteriaId { get; set; } // Link to the cafeteria
    }
}