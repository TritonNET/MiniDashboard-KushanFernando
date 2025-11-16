using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MiniDashboard.Client
{
    public class ProductClient : IProductService
    {
        private readonly HttpClient m_http;

        private readonly JsonSerializerOptions m_json;

        public ProductClient(HttpClient httpClient)
        {
            m_http = httpClient;

            m_json = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<Guid> AddProductAsync(Product product, CancellationToken cancellationToken)
        {
            var response = await m_http.PostAsJsonAsync("product", product, m_json, cancellationToken);

            await EnsureSuccess(response);

            var id = await response.Content.ReadFromJsonAsync<Guid>(m_json, cancellationToken);

            if (id == Guid.Empty)
                throw new InvalidOperationException("Server returned an empty GUID.");

            return id;
        }

        public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await m_http.DeleteAsync($"product/{id}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            await EnsureSuccess(response);
            return true;
        }

        public async Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await m_http.GetAsync($"product/{id}", cancellationToken);

            await EnsureSuccess(response);

            return await response.Content.ReadFromJsonAsync<Product>(m_json, cancellationToken)
                   ?? throw new InvalidOperationException("Server returned null product.");
        }

        public async Task<List<Product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken)
        {
            var url = BuildSearchUrl(filter);

            var response = await m_http.GetAsync(url, cancellationToken);

            await EnsureSuccess(response);

            return await response.Content.ReadFromJsonAsync<List<Product>>(m_json, cancellationToken)
                   ?? new List<Product>();
        }

        public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            if (product.ID == null)
                throw new ArgumentException("Product must contain a valid ID.");

            var response = await m_http.PutAsJsonAsync(
                $"product/{product.ID}", product, m_json, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            await EnsureSuccess(response);
            return true;
        }

        private static string BuildSearchUrl(ProductFilter filter)
        {
            var sb = new StringBuilder("product/search?");

            if (filter.Name != null && !string.IsNullOrWhiteSpace(filter.Name.Value))
            {
                sb.Append($"name={Uri.EscapeDataString(filter.Name.Value)}&");
                sb.Append($"type={(int)filter.Name.FilterType}&");
            }

            if (filter.MinPrice.HasValue)
                sb.Append($"minPrice={filter.MinPrice.Value}&");

            if (filter.MaxPrice.HasValue)
                sb.Append($"maxPrice={filter.MaxPrice.Value}&");

            var url = sb.ToString().TrimEnd('&', '?');

            return string.IsNullOrWhiteSpace(url)
                ? "product/search"
                : url;
        }

        private static async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException($"Request failed ({(int)response.StatusCode}): {body}", null, response.StatusCode);
        }
    }
}
