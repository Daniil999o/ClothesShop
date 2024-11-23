namespace ClothesShop.Models
{
    public class CartItem
    {
        public required ProductItem Product { get; set; }
        public int Quantity { get; set; }
        public required string Gender { get; set; }
    }
}
