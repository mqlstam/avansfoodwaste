using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Avans.FoodWaste.Core.Entities;

namespace Avans.FoodWaste.Core.Dtos
{
    public class UpdatePackageDto
    {
        [Required]
        public string Name { get; set; }
        public List<int> ExampleProductIds { get; set; }
        [Required]
        public DateTime PickupDateTime { get; set; }
        [Required]
        public DateTime LatestPickupTime { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public MealType MealType { get; set; }
        [Required]
        public int CafeteriaId { get; set; }
    }
}