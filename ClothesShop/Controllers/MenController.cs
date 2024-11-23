using Npgsql;
using ClothesShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClothesShop.Controllers
{
    public class MenController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly bool _isAdmin;

        private const string TABLE_NAME = "Men";

        public MenController(IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _isAdmin = appSettings.Value.IsAdmin;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(CardsModel cardsModel)
        {
            await Create(cardsModel.ItemToCreate);

            return Ok();
        }

        [HttpPost]
        public async Task Create(ProductItem newWomanItem)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"INSERT INTO {TABLE_NAME}Items(Brand, Model, Color, Size, Price, Avatar, Description) VALUES (@Brand, @Model, @Color, @Size, @Price, @Avatar, @Description) RETURNING Id", connection))
                {
                    command.Parameters.AddWithValue("Brand", newWomanItem.Brand);
                    command.Parameters.AddWithValue("Model", newWomanItem.Model);
                    command.Parameters.AddWithValue("Color", newWomanItem.Color);
                    command.Parameters.AddWithValue("Size", newWomanItem.Size);
                    command.Parameters.AddWithValue("Price", newWomanItem.Price);
                    command.Parameters.AddWithValue("Avatar", newWomanItem.Avatar);
                    command.Parameters.AddWithValue("Description", newWomanItem.Description);

                    int newItemId = (int)await command.ExecuteScalarAsync();

                    //return newItemId;
                }
            }
        }

        [HttpGet()]
        public async Task<List<ProductItem>> GetAll()
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"SELECT * FROM {TABLE_NAME}Items", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<ProductItem> womanItems = new List<ProductItem>();

                        while (await reader.ReadAsync())
                        {
                            womanItems.Add(new ProductItem
                            {
                                Id = reader.GetInt32(0),
                                Brand = reader.GetString(1),
                                Model = reader.GetString(2),
                                Color = reader.GetString(3),
                                Size = reader.GetString(4),
                                Price = (float)reader.GetDouble(5),
                                Avatar = reader.GetString(6),
                                Description = reader.GetString(7)
                            });
                        }

                        return womanItems;
                    }
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOne(int id)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"DELETE FROM {TABLE_NAME}Items WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("Id", id);

                    int affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows > 0)
                        return Ok();

                    return NotFound();
                }
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"TRUNCATE TABLE {TABLE_NAME}Items", connection)) //Используем TRUNCATE для быстрого удаления
                {
                    await command.ExecuteNonQueryAsync();

                    return Ok();
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            List<ProductItem> items = await GetAll();

            return View(new CardsModel
            {
                IsAdmin = _isAdmin,
                Items = items
            });
        }
    }
}