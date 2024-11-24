namespace ClothesShop.Models
{
    public class ProductItem
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public string Avatar { get; set; }
        public string Description { get; set; }
        public int ManufacturerId { get; set; }
    }
}
