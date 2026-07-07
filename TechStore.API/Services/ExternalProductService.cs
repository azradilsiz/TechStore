using System.Net.Http.Json;
using TechStore.API.DTOs.ExternalProducts;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class ExternalProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly string[] _categories =
        {
            "smartphones",
            "laptops",
            "mobile-accessories",
            "tablets"
        };

        public ExternalProductService(
            HttpClient httpClient,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _httpClient = httpClient;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<int> ImportProductsAsync()
        {
            var importedCount = 0;

            var existingProducts = await _productRepository.GetAllWithCategoryAsync();
            var existingProductNames = existingProducts
                .Select(product => product.Name.ToLower())
                .ToHashSet();

            foreach (var externalCategory in _categories)
            {
                var response = await _httpClient.GetFromJsonAsync<ExternalProductResponseDto>(
                    $"https://dummyjson.com/products/category/{externalCategory}");

                if (response == null || response.Products.Count == 0)
                {
                    continue;
                }

                var categoryName = MapCategoryName(externalCategory);
                var category = await GetOrCreateCategoryAsync(categoryName);

                foreach (var externalProduct in response.Products)
                {
                    if (existingProductNames.Contains(externalProduct.Title.ToLower()))
                    {
                        continue;
                    }

                    var product = new Product
                    {
                        CategoryId = category.Id,
                        Name = externalProduct.Title,
                        Description = externalProduct.Description,
                        Price = externalProduct.Price,
                        Stock = externalProduct.Stock,
                        ImageUrl = externalProduct.Thumbnail
                    };

                    await _productRepository.AddAsync(product);
                    existingProductNames.Add(product.Name.ToLower());
                    importedCount++;
                }
            }

            await _productRepository.SaveChangesAsync();

            return importedCount;
        }

        private async Task<Category> GetOrCreateCategoryAsync(string categoryName)
        {
            var categories = await _categoryRepository.GetAllAsync();

            var existingCategory = categories.FirstOrDefault(category =>
                category.Name.ToLower() == categoryName.ToLower());

            if (existingCategory != null)
            {
                return existingCategory;
            }

            var category = new Category
            {
                Name = categoryName
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return category;
        }

        private string MapCategoryName(string externalCategory)
        {
            return externalCategory switch
            {
                "smartphones" => "Telefon",
                "laptops" => "Laptop",
                "mobile-accessories" => "Aksesuar",
                "tablets" => "Tablet",
                _ => externalCategory
            };
        }
    }
}
