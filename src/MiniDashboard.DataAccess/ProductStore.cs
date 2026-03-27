using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using System.Text.Json;

namespace MiniDashboard.DataAccess
{
    public class ProductStore : BaseStore, IProductStore
    {
        private readonly List<tbl_product> m_products = new();

        public ProductStore(ILogger logger, IConfigProvider configProvider): base(logger, configProvider)
        {

        }

        public async Task AddProductAsync(tbl_product product, CancellationToken cancellationToken)
        {
            await EnsureLoadedAsync(cancellationToken);

            await m_lock.WaitAsync(cancellationToken);

            try
            {
                m_products.Add(product);

                await SaveAsync(cancellationToken);
            }
            finally
            {
                m_lock.Release();
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await EnsureLoadedAsync(cancellationToken);

            await m_lock.WaitAsync(cancellationToken);

            try
            {
                var item = m_products.FirstOrDefault(p => p.id == id);
                if (item == null)
                    return false;

                m_products.Remove(item);

                await SaveAsync(cancellationToken);
                
                return true;
            }
            finally
            {
                m_lock.Release();
            }
        }

        public async Task<bool> ExistsAsync(Guid? id, CancellationToken cancellationToken)
        {
            await EnsureLoadedAsync(cancellationToken);

            if (!id.HasValue)
                return false;

            await m_lock.WaitAsync(cancellationToken);
            try
            {
                return m_products.Any(p => p.id == id.Value);
            }
            finally
            {
                m_lock.Release();
            }
        }

        public async Task<tbl_product?> GetProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await EnsureLoadedAsync(cancellationToken);

            await m_lock.WaitAsync(cancellationToken);
            try
            {
                return m_products.FirstOrDefault(p => p.id == id);
            }
            finally
            {
                m_lock.Release();
            }
        }

        public async Task<List<tbl_product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken)
        {
            await EnsureLoadedAsync(cancellationToken);

            await m_lock.WaitAsync(cancellationToken);
            try
            {
                IEnumerable<tbl_product> query = m_products;

                // Name Filter
                if (filter?.Name != null && !string.IsNullOrWhiteSpace(filter.Name.Value))
                {
                    string val = filter.Name.Value;

                    query = filter.Name.FilterType switch
                    {
                        StringFilterType.Contains => query.Where(p => p.name?.Contains(val, StringComparison.OrdinalIgnoreCase) == true),

                        StringFilterType.StartsWith => query.Where(p => p.name?.StartsWith(val, StringComparison.OrdinalIgnoreCase) == true),

                        StringFilterType.EndsWith => query.Where(p => p.name?.EndsWith(val, StringComparison.OrdinalIgnoreCase) == true),

                        StringFilterType.Exact => query.Where(p => string.Equals(p.name, val, StringComparison.OrdinalIgnoreCase)),

                        _ => query
                    };
                }

                // Price Filter
                if (filter?.MinPrice.HasValue == true)
                    query = query.Where(p => p.price >= filter.MinPrice.Value);

                if (filter?.MaxPrice.HasValue == true)
                    query = query.Where(p => p.price <= filter.MaxPrice.Value);

                return query.ToList();
            }
            finally
            {
                m_lock.Release();
            }
        }

        public async Task<bool> UpdateProductAsync(tbl_product updated, CancellationToken cancellationToken)
        {
            await EnsureLoadedAsync(cancellationToken);

            await m_lock.WaitAsync(cancellationToken);
            try
            {
                var existing = m_products.FirstOrDefault(p => p.id == updated.id);
                if (existing == null)
                    return false;

                existing.name = updated.name;
                existing.description = updated.description;
                existing.price = updated.price;

                await SaveAsync(cancellationToken);

                return true;
            }
            finally
            {
                m_lock.Release();
            }
        }

        protected override async Task<bool> LoadAsync(Stream jsonStream, CancellationToken cancellationToken)
        {
            var loaded = await JsonSerializer.DeserializeAsync<List<tbl_product>>(jsonStream);

            if (loaded != null)
            {
                m_products.AddRange(loaded);

                m_logger.Info($"Loaded {loaded.Count} products from JSON file.");
            }
            else
            {
                m_logger.Info("JSON file exists but contained no valid products. Starting empty.");
            }

            return true;
        }

        protected override async Task<Stream> SerializeAsync(CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(
                stream,
                m_products,
                new JsonSerializerOptions { WriteIndented = true }
            );

            stream.Position = 0;

            return stream;
        }
    }
}
