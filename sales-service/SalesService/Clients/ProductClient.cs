using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SalesService.Clients
{
    public class ProductClient
    {
        private readonly HttpClient _http;

        public ProductClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<ProductDto?> GetProduct(int id)
        {
            try
            {
                var res = await _http.GetAsync($"api/products/{id}");
                if (!res.IsSuccessStatusCode) return null;

                return await res.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DecrementStock(List<DecrementStockDto> items)
        {
            try
            {
                var res = await _http.PatchAsJsonAsync("api/products/decrement-stock", items);
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }

    public class DecrementStockDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
