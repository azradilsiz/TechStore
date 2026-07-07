using TechStore.API.DTOs.Categories;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            List<Category> categories = await _categoryRepository.GetAllAsync();

            return categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            }).ToList();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            Category category = new Category
            {
                Name = dto.Name
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            category.Name = dto.Name;

            await _categoryRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            category.IsDeleted = true;
            await _categoryRepository.SaveChangesAsync();

            return true;
        }
    }
}
