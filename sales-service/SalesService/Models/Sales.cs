using System.ComponentModel.DataAnnotations;

namespace SalesService.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public List<SaleItem> Items { get; set; } = new();
    }
}
