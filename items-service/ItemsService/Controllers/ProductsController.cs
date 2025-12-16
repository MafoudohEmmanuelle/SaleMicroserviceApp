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
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");
            return Ok(product);
        }

        // POST create product
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

        // PUT update product
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

        // DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH decrement stock
        [HttpPatch("decrement-stock")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DecrementStock([FromBody] List<DecrementStockDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                    return NotFound($"Product {dto.ProductId} not found");

                if (product.Quantity < dto.Quantity)
                    return BadRequest($"Insufficient stock for product {product.Name}");

                product.Quantity -= dto.Quantity;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class DecrementStockDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
