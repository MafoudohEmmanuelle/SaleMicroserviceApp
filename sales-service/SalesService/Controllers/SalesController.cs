using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesService.Data;
using SalesService.Dtos;
using SalesService.Clients;
using SalesService.Models;
using System.Security.Claims;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/sales")]
    [Authorize] // Toutes les routes nécessitent un JWT
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProductClient _products;

        public SalesController(AppDbContext context, ProductClient products)
        {
            _context = context;
            _products = products;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(CreateSaleDto dto)
        {
            if (dto == null || dto.Items == null || !dto.Items.Any())
                return BadRequest("Sale must contain at least one item.");

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User id not found in token");

            var userId = int.Parse(userIdClaim);
            var userName = User.Identity?.Name ?? "Unknown";

            // Extract the JWT
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var sale = new Sale
            {
                CustomerName = dto.CustomerName,
                UserId = userId,
                UserName = userName,
                Items = new List<SaleItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _products.GetProduct(item.ProductId, token);
                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found in ItemsService!");

                // Decrement stock
                var stockUpdated = await _products.DecrementStock(item.ProductId, item.Quantity, token);
                if (!stockUpdated)
                    return BadRequest($"Insufficient stock for product ID {item.ProductId}");

                var saleItem = new SaleItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? "Unknown",
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                // FIX #1: Add item to sale
                sale.Items.Add(saleItem);

                // FIX #2: Update total price
                total += saleItem.UnitPrice * saleItem.Quantity;
            }

            sale.TotalAmount = total;

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return Ok(sale);
        }

        // GET ALL SALES
        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            var sales = await _context.Sales
                .Include(s => s.Items)
                .ToListAsync();

            return Ok(sales);
        }
    }
}
