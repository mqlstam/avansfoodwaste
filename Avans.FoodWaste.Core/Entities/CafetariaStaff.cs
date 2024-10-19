namespace Avans.FoodWaste.Core.Entities
{
    public class CafeteriaStaff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeNumber { get; set; }
        public int CafeteriaId { get; set; }
        public Cafeteria Cafeteria { get; set; } // Navigation property
    }
}