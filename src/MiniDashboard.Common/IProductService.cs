using MiniDashboard.Common.Models;

namespace MiniDashboard.Common
{
    public interface IProductService
    {
        Task<Guid> AddProductAsync(Product product, CancellationToken cancellationToken);

        Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);
        
        Task<Product> GetProductAsync(Guid id, CancellationToken cancellationToken);
        
        Task<List<Product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken);
        
        Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken);
    }
}
