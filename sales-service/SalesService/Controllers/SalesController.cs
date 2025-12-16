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
                return BadRequest("La vente doit contenir au moins un item.");

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User id not found in token");

            var userId = int.Parse(userIdClaim);
            var userName = User.Identity?.Name ?? "Unknown";

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
                var product = await _products.GetProduct(item.ProductId);
                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found in ItemsService!");

                var saleItem = new SaleItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? "Unknown",
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                sale.Items.Add(saleItem);
                total += saleItem.UnitPrice * saleItem.Quantity;
            }

            sale.TotalAmount = total;

            // Décrémenter les stocks dans ItemsService
            var stockUpdate = sale.Items.Select(i => new DecrementStockDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            var stockUpdated = await _products.DecrementStock(stockUpdate);
            if (!stockUpdated)
                return BadRequest("Failed to update stock. Please check product quantities.");

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
