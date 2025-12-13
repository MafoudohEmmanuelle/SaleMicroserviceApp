using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItemsService.Data;
using ItemsService.Models;
using ItemsService.Dtos;

namespace ItemsService.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize] // all endpoints require login by default
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all products (all authorized users)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // POST create new product (only Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // PUT update product (only Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");

            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;
            if (dto.Quantity.HasValue)
                product.Quantity = dto.Quantity.Value;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        // ✅ Allow anonymous access for SalesService lookups
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");
            return Ok(product);
        }

        // DELETE product (only Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 response
        }

        // POST api/products/decrement
        [HttpPost("decrement")]
        public async Task<IActionResult> DecrementStock(DecrementStockDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
                return NotFound($"Product {dto.ProductId} not found");

            if (product.Quantity < dto.Quantity)
                return BadRequest("Insufficient stock");

            product.Quantity -= dto.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                product.Id,
                product.Name,
                product.Quantity
            });
        }

    }
}