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

        public async Task<ProductDto?> GetProduct(int id, string? bearerToken = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/products/{id}");
                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                var res = await _http.SendAsync(request);
                if (!res.IsSuccessStatusCode) return null;

                return await res.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DecrementStock(List<DecrementStockDto> items, string? bearerToken = null)
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), "api/products/decrement-stock")
                {
                    Content = JsonContent.Create(items)
                };
                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                var res = await _http.SendAsync(request);
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
