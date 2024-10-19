using System;
using System.Collections.Generic;

namespace Avans.FoodWaste.Core.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> ExampleProductIds { get; set; } = new(); 
        public DateTime PickupDateTime { get; set; }
        public DateTime LatestPickupTime { get; set; }
        public bool IsAdultPackage { get; set; }
        public decimal Price { get; set; }
        public MealType MealType { get; set; }
        public int? ReservedById { get; set; }
        public Student? ReservedBy { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        public NoShowStatus NoShowStatus { get; set; }
        public int CafeteriaId { get; set; }
        public Cafeteria Cafeteria { get; set; }


    }

    public enum MealType
    {
        Bread,
        HotDinner,
        Drinks,
        Other
    }

    public enum ReservationStatus
    {
        Available,
        Reserved,
        PickedUp
    }

    public enum NoShowStatus
    {
        None,
        NoShow
    }
}