using System.Net.Http.Json;
using TechStore.API.DTOs.ExternalProducts;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;
using TechStore.API.Resources;

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
            Dictionary<string, Product> existingProductsByName = existingProducts
                .ToDictionary(product => product.Name.ToLowerInvariant());

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
                    ProductTranslation? translation = ProductTranslations.Find(externalProduct.Title);
                    string displayName = translation?.DisplayName ?? externalProduct.Title;
                    string description = translation?.Description ?? externalProduct.Description;

                    Product? existingProduct = FindExistingProduct(existingProductsByName, externalProduct.Title, displayName);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = displayName;
                        existingProduct.Description = description;
                        existingProductsByName[displayName.ToLowerInvariant()] = existingProduct;
                        continue;
                    }

                    Product product = new Product
                    {
                        CategoryId = category.Id,
                        Name = displayName,
                        Description = description,
                        Price = externalProduct.Price,
                        Stock = externalProduct.Stock,
                        ImageUrl = externalProduct.Thumbnail
                    };

                    await _productRepository.AddAsync(product);
                    existingProductsByName[product.Name.ToLowerInvariant()] = product;
                    importedCount++;
                }
            }

            await _productRepository.SaveChangesAsync();

            return importedCount;
        }

        public async Task<int> LocalizeExistingProductsAsync(bool onlyUnlocalizedDescriptions = false)
        {
            List<Product> products = await _productRepository.GetAllWithCategoryAsync();
            int updatedCount = 0;

            foreach (Product product in products)
            {
                ProductTranslation? translation = ProductTranslations.Find(product.Name);
                if (translation == null ||
                    (onlyUnlocalizedDescriptions && !LooksLikeEnglishDemoDescription(product.Description)) ||
                    (product.Name == translation.DisplayName && product.Description == translation.Description))
                {
                    continue;
                }

                product.Name = translation.DisplayName;
                product.Description = translation.Description;
                updatedCount++;
            }

            if (updatedCount > 0)
            {
                await _productRepository.SaveChangesAsync();
            }

            return updatedCount;
        }

        private static bool LooksLikeEnglishDemoDescription(string description)
        {
            string normalizedDescription = description.TrimStart();
            return normalizedDescription.StartsWith("The ", StringComparison.OrdinalIgnoreCase) ||
                normalizedDescription.StartsWith("It ", StringComparison.OrdinalIgnoreCase);
        }

        private static Product? FindExistingProduct(
            IReadOnlyDictionary<string, Product> existingProducts,
            string sourceName,
            string displayName)
        {
            if (existingProducts.TryGetValue(sourceName.ToLowerInvariant(), out Product? sourceProduct))
            {
                return sourceProduct;
            }

            existingProducts.TryGetValue(displayName.ToLowerInvariant(), out Product? displayProduct);
            return displayProduct;
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
