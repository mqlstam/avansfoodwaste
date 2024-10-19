using System.Collections.Generic;

namespace Avans.FoodWaste.Core.Entities
{
    public class Cafeteria
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string LocationIdentifier { get; set; }
        public bool HotMealsAvailable { get; set; }
        public string OperatingHours { get; set; }
        public List<CafeteriaStaff> Staff { get; set; } = new();
        public List<Package> Packages { get; set; } = new(); 
    }
}