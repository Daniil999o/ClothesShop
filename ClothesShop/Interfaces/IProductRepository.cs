using ClothesShop.Models;

namespace ClothesShop.Interfaces
{
    public interface IProductRepository<T> where T : ProductItem
    {
        Task<T> GetOneAsync(int id);
        Task<List<T>> GetAllAsync();
        Task DeleteOneAsync(int id);
        Task DeleteAllAsync();
        Task UpdateAsync(T product);
        Task<int> AddAsync(T product);
    }
}
