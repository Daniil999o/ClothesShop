namespace ClothesShop.Cart
{
    public class CartSaveItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public required string Gender { get; set; }
    }
}
