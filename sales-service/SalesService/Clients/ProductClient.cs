using System.Net.Http.Headers;
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

        // GET /api/products/{id} from ItemsService with token forwarding
        public async Task<ProductDto?> GetProduct(int id, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/products/{id}");
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var res = await _http.SendAsync(request);
                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine($"ProductClient GET failed: {res.StatusCode}");
                    return null;
                }

                return await res.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProductClient exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DecrementStock(int productId, int quantity, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/products/decrement");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(new
            {
                ProductId = productId,
                Quantity = quantity
            });

            var res = await _http.SendAsync(request);
            return res.IsSuccessStatusCode;
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}