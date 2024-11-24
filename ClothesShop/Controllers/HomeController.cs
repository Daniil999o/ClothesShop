using Npgsql;
using System.Diagnostics;
using ClothesShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClothesShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;
        private readonly bool _isAdmin;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _configuration = configuration;
            _isAdmin = appSettings.Value.IsAdmin;
        }

        public async Task<IActionResult> Index()
        {
            return View(new ManufacturerModel
            {
                IsAdmin = _isAdmin,
                Manufacturers = await GetAllManufacturers()
            });
        }

        public IActionResult Card()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CreateManufacturer(ManufacturerModel manufacturerModel)
        {
            await CreateManufacturerItem(manufacturerModel.Manufacturer);

            return Ok();
        }

        [HttpPost]
        public async Task CreateManufacturerItem(Manufacturer item)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"INSERT INTO Manufacturers(Name, Country, Address) VALUES (@Name, @Country, @Address) RETURNING Id", connection))
                {
                    command.Parameters.AddWithValue("Name", item.Name);
                    command.Parameters.AddWithValue("Country", item.Country);
                    command.Parameters.AddWithValue("Address", item.Address);

                    int newItemId = (int)await command.ExecuteScalarAsync();

                    //return newItemId;
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOneManufacturer(int id)
        {
            string connectionString = _configuration.GetConnectionString("PostgreSQLConnection");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand($"DELETE FROM Manufacturers WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("Id", id);

                    int affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows > 0)
                        return Ok();

                    return NotFound();
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
