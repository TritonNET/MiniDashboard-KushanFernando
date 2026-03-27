using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using MiniDashboard.DataAccess;

namespace MiniDashboard.Services
{
    public class ProductService : IProductService
    {
        private const int MAX_PRODUCT_PRICE = 100000;

        private readonly IProductStore m_productStore;
        private readonly ILogger m_logger;

        public ProductService(ILogger logger, IProductStore productStore)
        {
            m_logger = logger;
            m_productStore = productStore;
        }

        public async Task<Guid> AddProductAsync(Product product, CancellationToken cancellationToken)
        {
            m_logger.Verbose("Add product", product);

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentNullException(nameof(product.Name));

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than 0", nameof(product.Price));

            if (product.Price > MAX_PRODUCT_PRICE)
                throw new ArgumentException($"Invalid product price: {product.Price}. Must be <= {MAX_PRODUCT_PRICE}");

            if (!product.ID.HasValue)
                product.ID = Guid.NewGuid();
            else if (await m_productStore.ExistsAsync(product.ID, cancellationToken))
                throw new InvalidOperationException($"Product with id {product.ID} already exists.");

            var entity = new tbl_product
            {
                id = product.ID.Value,
                name = product.Name,
                description = product.Description,
                price = product.Price
            };

            try
            {
                await m_productStore.AddProductAsync(entity, cancellationToken);

                m_logger.Debug("Product saved successfully.", product);

                return entity.id;
            }
            catch (Exception ex)
            {
                m_logger.Error("Product saving failed.", ex, product);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            m_logger.Debug($"Delete product: {id}");

            try
            {
                bool status = await m_productStore.DeleteProductAsync(id, cancellationToken);

                if (!status)
                    m_logger.Debug($"Product not found for deletion: {id}");

                return status;
            }
            catch (Exception ex)
            {
                m_logger.Error("Product deletion failed.", ex, id);
                throw;
            }
        }

        public async Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken)
        {
            m_logger.Verbose("Get product", id);

            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            try
            {
                var tbl = await m_productStore.GetProductAsync(id, cancellationToken);

                if (tbl == null)
                {
                    var msg = $"Product not found: {id}";
                    m_logger.Debug(msg);
                    throw new InvalidOperationException(msg);
                }

                var product = new Product
                {
                    ID = tbl.id,
                    Name = tbl.name,
                    Description = tbl.description,
                    Price = tbl.price
                };

                m_logger.Debug("Product retrieved", product);

                return product;
            }
            catch (Exception ex)
            {
                m_logger.Error("Failed to retrieve product", ex, id);
                throw;
            }
        }

        public async Task<List<Product>> GetProductsAsync(ProductFilter filter, int page, int pagesize, CancellationToken cancellationToken)
        {
            m_logger.Verbose("Get products", filter);

            try
            {
                var tblProducts = await m_productStore.GetProductsAsync(filter, cancellationToken);

                var products = tblProducts.Skip((page - 1) * pagesize).Take(pagesize).Select(tbl => new Product
                {
                    ID = tbl.id,
                    Name = tbl.name,
                    Description = tbl.description,
                    Price = tbl.price
                }).ToList();

                m_logger.Debug("Products retrieved", products);

                return products;
            }
            catch (Exception ex)
            {
                m_logger.Error("Failed to retrieve products", ex, filter);
                throw;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            m_logger.Verbose("Update product", product);

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!product.ID.HasValue || product.ID == Guid.Empty)
                throw new ArgumentException("Product must have a valid ID.", nameof(product.ID));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentNullException(nameof(product.Name));

            if (product.Price <= 0 || product.Price > MAX_PRODUCT_PRICE)
                throw new ArgumentException($"Invalid product price: {product.Price}. Must be > 0 and <= {MAX_PRODUCT_PRICE}");

            try
            {
                if (!await m_productStore.ExistsAsync(product.ID, cancellationToken))
                    throw new InvalidOperationException($"Product with ID {product.ID} does not exist.");

                var tbl = new tbl_product
                {
                    id = product.ID.Value,
                    name = product.Name,
                    description = product.Description,
                    price = product.Price
                };

                bool status = await m_productStore.UpdateProductAsync(tbl, cancellationToken);

                if (status)
                    m_logger.Debug("Product updated successfully.", product);

                return status;
            }
            catch (Exception ex)
            {
                m_logger.Error("Product update failed.", ex, product);
                throw;
            }
        }
    }
}
