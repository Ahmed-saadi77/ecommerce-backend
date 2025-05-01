using E_commerce.DTOs;
using E_commerce.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_commerce.Controllers
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
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("store/{storeId}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByStoreId(int storeId)
        {
            var products = await _productService.GetProductsByStoreIdAsync(storeId);
            return Ok(products);
        }

        [HttpGet("store/{storeId}/featured")]
        public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts(int storeId)
        {
            var products = await _productService.GetFeaturedProductsAsync(storeId);
            return Ok(products);
        }

        [HttpGet("store/{storeId}/archived")]
        public async Task<ActionResult<List<ProductDto>>> GetArchivedProducts(int storeId)
        {
            var products = await _productService.GetArchivedProductsAsync(storeId);
            return Ok(products);
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<List<ProductDto>>> GetFilteredProducts(
            [FromQuery] int? categoryId,
            [FromQuery] int? colorId,
            [FromQuery] int? sizeId,
            [FromQuery] bool? isFeatured)
        {
            var products = await _productService.GetFilteredProductsAsync( categoryId, colorId, sizeId, isFeatured);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto productDto)
        {
            var newProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto productDto)
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
