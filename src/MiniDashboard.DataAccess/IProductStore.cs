using MiniDashboard.Common.Models;

namespace MiniDashboard.DataAccess
{
    public interface IProductStore
    {
        Task AddProductAsync(tbl_product product, CancellationToken cancellationToken);

        Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(Guid? id, CancellationToken cancellationToken);

        Task<tbl_product?> GetProductAsync(Guid id, CancellationToken cancellationToken);

        Task<List<tbl_product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken);

        Task<bool> UpdateProductAsync(tbl_product updated, CancellationToken cancellationToken);
    }
}
