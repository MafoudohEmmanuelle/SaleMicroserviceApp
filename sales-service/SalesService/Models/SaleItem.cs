using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SalesService.Models
{
    public class SaleItem
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public int SaleId { get; set; }

        // ✅ Prevent infinite loop during JSON serialization
        [JsonIgnore]
        public Sale Sale { get; set; } = null!;
    }
}