using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Common;
using MiniDashboard.Common.Models;

namespace MiniDashboard.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ApiControllerBase
    {
        private readonly IProductService m_productService;

        public ProductController(IProductService productService)
        {
            m_productService = productService;
        }
        // /product?page=12&pagesize=234
        [HttpGet()]
        public async Task<ActionResult<List<Product>>> GetAll([FromQuery]int page, [FromQuery]int pagesize, CancellationToken ct)
            => await HandleAsync(() => m_productService.GetProductsAsync(new ProductFilter(), page, pagesize, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Product>> Get(Guid id, CancellationToken ct)
            => await HandleAsync(() => m_productService.GetProductAsync(id, ct));

        [HttpGet("search")]
        public async Task<ActionResult<List<Product>>> Search(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] StringFilterType type = StringFilterType.Contains,
            CancellationToken ct = default)
        {
            var filter = new ProductFilter
            {
                Name = name == null ? null : new StringFilter { Value = name, FilterType = type },
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return await HandleAsync(() => m_productService.GetProductsAsync(filter, 0, 1000, ct));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Add([FromBody] Product product, CancellationToken ct)
        {
            return await HandleAsync(() => m_productService.AddProductAsync(product, ct));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] Product product, CancellationToken ct)
        {
            if (product.ID != id)
                return BadRequest("Product ID in URI and body must match.");

            return await HandleAsync(() => m_productService.UpdateProductAsync(product, ct));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
            => await HandleAsync(() => m_productService.DeleteProductAsync(id, ct));
    }
}
