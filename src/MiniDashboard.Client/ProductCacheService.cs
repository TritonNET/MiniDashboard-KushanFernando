using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using System.Collections.Concurrent;

namespace MiniDashboard.Client
{
    public class ProductCacheService : IProductService
    {
        private readonly IProductService m_backend;

        private readonly ConcurrentDictionary<Guid, Product> m_cache = new ConcurrentDictionary<Guid, Product>();

        private bool m_cacheLoaded = false;
        private readonly SemaphoreSlim m_cacheLock = new SemaphoreSlim(1, 1);

        public ProductCacheService(IProductService productService)
        {
            m_backend = productService;
        }

        private async Task EnsureCacheLoaded(CancellationToken ct)
        {
            if (m_cacheLoaded)
                return;

            await m_cacheLock.WaitAsync(ct);
            try
            {
                if (m_cacheLoaded)
                    return;

                var products = await m_backend.GetProductsAsync(new ProductFilter(), ct);

                m_cache.Clear();

                foreach (var p in products)
                    m_cache[p.ID!.Value] = p;

                m_cacheLoaded = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                m_cacheLock.Release();
            }
        }

        public async Task<Guid> AddProductAsync(Product product, CancellationToken cancellationToken)
        {
            var id = await m_backend.AddProductAsync(product, cancellationToken);

            var created = await m_backend.GetProductAsync(id, cancellationToken);
            m_cache[id] = created;

            return id;
        }

        public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            var success = await m_backend.DeleteProductAsync(id, cancellationToken);

            if (success)
                m_cache.TryRemove(id, out _);

            return success;
        }

        public async Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await EnsureCacheLoaded(cancellationToken);

            if (m_cache.TryGetValue(id, out var cached))
                return cached;

            var product = await m_backend.GetProductAsync(id, cancellationToken);
            m_cache[id] = product;

            return product;
        }

        public async Task<List<Product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken)
        {
            await EnsureCacheLoaded(cancellationToken);

            IEnumerable<Product> query = m_cache.Values;

            if (filter.Name is not null && !string.IsNullOrWhiteSpace(filter.Name.Value))
            {
                var val = filter.Name.Value;
                var type = filter.Name.FilterType;

                query = type switch
                {
                    StringFilterType.Contains =>
                        query.Where(p => p.Name.Contains(val, StringComparison.OrdinalIgnoreCase)),

                    StringFilterType.StartsWith =>
                        query.Where(p => p.Name.StartsWith(val, StringComparison.OrdinalIgnoreCase)),

                    StringFilterType.EndsWith =>
                        query.Where(p => p.Name.EndsWith(val, StringComparison.OrdinalIgnoreCase)),

                    StringFilterType.Exact =>
                        query.Where(p => p.Name.Equals(val, StringComparison.OrdinalIgnoreCase)),

                    _ => query
                };
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            return query.ToList();
        }

        public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            var success = await m_backend.UpdateProductAsync(product, cancellationToken);

            if (success && product.ID.HasValue)
            {
                var updated = await m_backend.GetProductAsync(product.ID.Value, cancellationToken);
                m_cache[product.ID.Value] = updated;
            }

            return success;
        }
    }
}
