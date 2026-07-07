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
            int importedCount = 0;

            List<Product> existingProducts = await _productRepository.GetAllWithCategoryAsync();
            HashSet<string> existingProductNames = existingProducts
                .Select(product => product.Name.ToLower())
                .ToHashSet();

            foreach (string externalCategory in _categories)
            {
                ExternalProductResponseDto? response = await _httpClient.GetFromJsonAsync<ExternalProductResponseDto>(
                    $"https://dummyjson.com/products/category/{externalCategory}");

                if (response == null || response.Products.Count == 0)
                {
                    continue;
                }

                string categoryName = MapCategoryName(externalCategory);
                Category category = await GetOrCreateCategoryAsync(categoryName);

                foreach (ExternalProductDto externalProduct in response.Products)
                {
                    if (existingProductNames.Contains(externalProduct.Title.ToLower()))
                    {
                        continue;
                    }

                    Product product = new Product
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
            List<Category> categories = await _categoryRepository.GetAllAsync();

            Category? existingCategory = categories.FirstOrDefault(category =>
                category.Name.ToLower() == categoryName.ToLower());

            if (existingCategory != null)
            {
                return existingCategory;
            }

            Category category = new Category
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
