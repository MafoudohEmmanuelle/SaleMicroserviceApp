using System.ComponentModel.DataAnnotations;

namespace ItemsService.Dtos
{
    public class CreateProductDto
    {
        [Required] public string Name { get; set; } = null!;
        [Required] public string Description { get; set; } = null!;
        [Required] [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
        [Required] [Range(0, int.MaxValue)] public int Quantity { get; set; }
    }

    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Range(0.01, double.MaxValue)] public decimal? Price { get; set; }
        [Range(0, int.MaxValue)] public int? Quantity { get; set; }
    }
}
