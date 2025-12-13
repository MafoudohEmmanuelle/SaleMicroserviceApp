using System.ComponentModel.DataAnnotations;

namespace SalesService.Dtos
{
    public class CreateSaleDto
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public List<SaleItemDto> Items { get; set; } = new();
    }
}
