namespace Avans.FoodWaste.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ContainsAlcohol { get; set; }
        public string? PhotoUrl { get; set; }
        public string ProductType { get; set; }
    }
}