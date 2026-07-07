using TechStore.API.DTOs.Products;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            List<Product> products = await _productRepository.GetAllWithCategoryAsync();

            return products.Select(product => new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : string.Empty,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            }).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            Product? product = await _productRepository.GetByIdWithCategoryAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : string.Empty,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            };
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            Product product = new Product
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            ProductDto? createdProduct = await GetProductByIdAsync(product.Id);

            return createdProduct!;
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            Product? product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            product.CategoryId = dto.CategoryId;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;

            await _productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            Product? product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            product.IsDeleted = true;
            await _productRepository.SaveChangesAsync();

            return true;
        }
    }
}
