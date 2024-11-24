using Npgsql;
using ClothesShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ClothesShop.Cart;
using Microsoft.AspNetCore.Http;

namespace ClothesShop.Controllers
{
    public class CardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly bool _isAdmin;

        public CardController(IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _isAdmin = appSettings.Value.IsAdmin;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(CardsModel cardsModel)
        {
            await Update(cardsModel.Gender, cardsModel.ItemToCreate);
            return Ok();
        }

        [HttpPost]
        public IActionResult AddToCart(int id, string gender)
        {
            CartSaver.AddToCart(HttpContext, id, gender, 1);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Update(string gender, ProductItem updatedWomanItem)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"UPDATE {gender}Items SET Brand = @Brand, Model = @Model, Color = @Color, Size = @Size, Price = @Price, Avatar = @Avatar, Description = @Description, ManufacturerId = @ManufacturerId WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("Id", updatedWomanItem.Id);
                    command.Parameters.AddWithValue("Brand", updatedWomanItem.Brand);
                    command.Parameters.AddWithValue("Model", updatedWomanItem.Model);
                    command.Parameters.AddWithValue("Color", updatedWomanItem.Color);
                    command.Parameters.AddWithValue("Size", updatedWomanItem.Size);
                    command.Parameters.AddWithValue("Price", updatedWomanItem.Price);
                    command.Parameters.AddWithValue("Avatar", updatedWomanItem.Avatar);
                    command.Parameters.AddWithValue("Description", updatedWomanItem.Description);
                    command.Parameters.AddWithValue("ManufacturerId", updatedWomanItem.ManufacturerId);

                    int affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows > 0)
                        return Ok();

                    return NotFound();
                }
            }
        }

        [HttpGet("{gender}/{id}")]
        public async Task<IActionResult> Index(string gender, int id)
        {
            ProductItem item = await GetOne(gender, id);

            if (item == null)
            {
                return NotFound();
            }

            return View(new CardsModel
            {
                ItemToCreate = item,
                IsAdmin = _isAdmin,
                Gender = gender,
                Manufacturers = await GetAllManufacturers(),
                Manufacturer = await GetOneManufacturer(item.ManufacturerId)
            });
        }

        [HttpGet]
        public async Task<ProductItem> GetOne(string gender, int id)
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

        [HttpGet]
        public async Task<Manufacturer> GetOneManufacturer(int id)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"SELECT * FROM Manufacturers WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Manufacturer
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Country = reader.GetString(2),
                                Address = reader.GetString(3)
                            };
                        }

                        return new Manufacturer
                        {
                            Id = -1,
                            Name = "Error",
                            Country = "Error",
                            Address = "Error"
                        };
                    }
                }
            }
        }

        [HttpGet()]
        public async Task<List<Manufacturer>> GetAllManufacturers()
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"SELECT * FROM Manufacturers", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<Manufacturer> manufacturers = new List<Manufacturer>();

                        while (await reader.ReadAsync())
                        {
                            manufacturers.Add(new Manufacturer
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Country = reader.GetString(2),
                                Address = reader.GetString(3)
                            });
                        }

                        return manufacturers;
                    }
                }
            }
        }
    }
}