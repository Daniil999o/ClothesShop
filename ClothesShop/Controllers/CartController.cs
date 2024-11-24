using Npgsql;
using ClothesShop.Cart;
using ClothesShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClothesShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;


        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpDelete]
        public IActionResult RemoveFromCart(int productId, string gender)
        {
            CartSaver.RemoveFromCart(HttpContext, productId, gender);

            return Ok();
        }


        [HttpGet]
        public async Task<List<CartItem>> GetCartItems()
        {
            var cartItems = new List<CartItem>();
            var saveItems = CartSaver.GetCartItems(HttpContext);

            foreach (var x in saveItems)
            {
                ProductItem item = await GetProduct(x.Gender, x.Id);

                cartItems.Add(new CartItem { Gender = x.Gender, Product = item, Quantity = x.Quantity });
            }

            return cartItems;
        }

        public async Task<IActionResult> Index()
        {
            return View(new CartModel
            {
                CartItems = await GetCartItems()
            });
        }

        private async Task<ProductItem> GetProduct(string gender, int id)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"SELECT * FROM {gender}Items WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProductItem
                            {
                                Id = reader.GetInt32(0),
                                Brand = reader.GetString(1),
                                Model = reader.GetString(2),
                                Color = reader.GetString(3),
                                Size = reader.GetString(4),
                                Price = (float)reader.GetDouble(5),
                                Avatar = reader.GetString(6),
                                Description = reader.GetString(7),
                                ManufacturerId = reader.GetInt32(8)
                            };
                        }

                        return new ProductItem
                        {
                            Id = -1,
                            Brand = "Error"
                        };
                    }
                }
            }
        }
    }
}
