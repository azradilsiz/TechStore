using Microsoft.AspNetCore.Mvc;
using TechStore.API.DTOs.Products;
using TechStore.API.Services;

namespace TechStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ExternalProductService _externalProductService;

        public ProductsController(
            ProductService productService,
            ExternalProductService externalProductService)
        {
            _productService = productService;
            _externalProductService = externalProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            List<ProductDto> products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            ProductDto? product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("import-external")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportExternalProducts()
        {
            int importedCount = await _externalProductService.ImportProductsAsync();

            return Ok(new
            {
                Message = "Dış kaynaktaki ürünler başarıyla içe aktarıldı.",
                ImportedCount = importedCount
            });
        }

        [HttpPost("localize-demo-products")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> LocalizeDemoProducts()
        {
            int updatedCount = await _externalProductService.LocalizeExistingProductsAsync();

            return Ok(new
            {
                Message = "Demo ürün bilgileri Türkçe olarak güncellendi.",
                UpdatedCount = updatedCount
            });
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Ürün adı boş bırakılamaz.");
            }

            if (dto.Price <= 0)
            {
                return BadRequest("Ürün fiyatı sıfırdan büyük olmalıdır.");
            }

            if (dto.Stock < 0)
            {
                return BadRequest("Ürün stok miktarı negatif olamaz.");
            }

            ProductDto product = await _productService.CreateProductAsync(dto);

            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
        }

        [HttpPut("{productId}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Ürün adı boş bırakılamaz.");
            }

            if (dto.Price <= 0)
            {
                return BadRequest("Ürün fiyatı sıfırdan büyük olmalıdır.");
            }

            if (dto.Stock < 0)
            {
                return BadRequest("Ürün stok miktarı negatif olamaz.");
            }

            bool result = await _productService.UpdateProductAsync(productId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{productId}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            bool result = await _productService.DeleteProductAsync(productId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
