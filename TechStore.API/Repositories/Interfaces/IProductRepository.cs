using TechStore.API.Entities;

namespace TechStore.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllWithCategoryAsync();

        Task<Product?> GetByIdWithCategoryAsync(int id);

        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);

        void Delete(Product product);

        Task SaveChangesAsync();
    }
}
