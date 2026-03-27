using DaifukuOceania.Test.Services.Models;

namespace DaifukuOceania.Test.Services
{
    public interface IProductService
    {
        bool AddProduct(Product product);

        List<Product> GetProducts();

        bool DeleteProduct(Guid productID);
    }
}
