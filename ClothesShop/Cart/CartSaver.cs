using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClothesShop.Cart
{
    public static class CartSaver
    {
        public static void AddToCart(HttpContext httpContext, int productId, string gender, int quantity)
        {
            List<CartSaveItem> cart = GetCartFromSession(httpContext);

            var existingCartItem = cart.Find(item => item.Id == productId && item.Gender == gender);

            if (existingCartItem != null)
                existingCartItem.Quantity += quantity;
            else
                cart.Add(new CartSaveItem { Id = productId, Quantity = quantity, Gender = gender });

            SaveCartToSession(httpContext, cart);
        }

        public static void RemoveFromCart(HttpContext httpContext, int productId, string gender)
        {
            List<CartSaveItem> cart = GetCartFromSession(httpContext);

            cart.RemoveAll(item => item.Id == productId && item.Gender == gender);

            SaveCartToSession(httpContext, cart);
        }

        public static List<CartSaveItem> GetCartItems(HttpContext httpContext) => GetCartFromSession(httpContext);

        private static List<CartSaveItem> GetCartFromSession(HttpContext httpContext)
        {
            var cartJson = httpContext.Session.GetString("Cart");

            var cart = new List<CartSaveItem>();

            if (cartJson != null)
                cart = JsonConvert.DeserializeObject<List<CartSaveItem>>(cartJson);

            if (cart == null) cart = new List<CartSaveItem>();

            CartInfo.CartCount = GetCartCount(cart);

            return cart;

        }

        private static int GetCartCount(List<CartSaveItem> items) => items.Sum(x => x.Quantity);

        private static void SaveCartToSession(HttpContext httpContext, List<CartSaveItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            httpContext.Session.SetString("Cart", cartJson);

            CartInfo.CartCount = GetCartCount(cart);
        }
    }
}
