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

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Product name cannot be empty.");
            }

            if (dto.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero.");
            }

            if (dto.Stock < 0)
            {
                return BadRequest("Product stock cannot be negative.");
            }

            var product = await _productService.CreateProductAsync(dto);

            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Product name cannot be empty.");
            }

            if (dto.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero.");
            }

            if (dto.Stock < 0)
            {
                return BadRequest("Product stock cannot be negative.");
            }

            var result = await _productService.UpdateProductAsync(productId, dto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _productService.DeleteProductAsync(productId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
