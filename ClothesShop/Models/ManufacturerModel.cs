namespace ClothesShop.Models
{
    public class ManufacturerModel
    {
        public bool IsAdmin { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public List<Manufacturer> Manufacturers { get; set; }
    }
}
