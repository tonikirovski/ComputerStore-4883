using Microsoft.AspNetCore.Mvc;
using ComputerStoreWebApi.DTOs;
using ComputerStoreWebApi.Models;
using ComputerStoreWebApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerStoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryIds = p.Categories.Select(c => c.Id).ToList(),
                Quantity = p.Quantity
            }).ToList();

            return Ok(productDtos);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryIds = product.Categories.Select(c => c.Id).ToList(),
                Quantity = product.Quantity
            };

            return Ok(productDto);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var productCategories = categories.Where(c => productDto.CategoryIds.Contains(c.Id)).ToList();

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Categories = productCategories,
                Quantity = productDto.Quantity
            };

            await _productRepository.AddProductAsync(product);

            productDto.Id = product.Id;

            return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var productCategories = categories.Where(c => productDto.CategoryIds.Contains(c.Id)).ToList();

            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Categories = productCategories,
                Quantity = productDto.Quantity
            };

            await _productRepository.UpdateProductAsync(product);

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteProductAsync(id);

            return NoContent();
        }
    }
}
