using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using MiniDashboard.DataAccess;
using Moq;

namespace MiniDashboard.Services.Test
{
    public class ProductServiceTest
    {
        private Mock<IProductStore> m_store;
        private Mock<ILogger> m_logger;
        private ProductService m_service;

        private readonly CancellationToken _ct = CancellationToken.None;

        [SetUp]
        public void Setup()
        {
            m_store = new Mock<IProductStore>(MockBehavior.Strict);
            m_logger = new Mock<ILogger>(MockBehavior.Loose);

            m_service = new ProductService(m_logger.Object, m_store.Object);
        }

        [Test]
        public void AddProduct_ShouldThrow_WhenProductIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => m_service.AddProductAsync(null, _ct));
        }

        [Test]
        public void AddProduct_ShouldThrow_WhenNameIsEmpty()
        {
            var product = new Product { Price = 10 };

            Assert.ThrowsAsync<ArgumentNullException>(() => m_service.AddProductAsync(product, _ct));
        }

        [Test]
        public void AddProduct_ShouldThrow_WhenPriceInvalid()
        {
            var product = new Product { Name = "A", Price = 0 };

            Assert.ThrowsAsync<ArgumentException>(() => m_service.AddProductAsync(product, _ct));
        }

        [Test]
        public void AddProduct_ShouldThrow_WhenProductAlreadyExists()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, Name = "Test", Price = 10 };

            m_store.Setup(x => x.ExistsAsync(id, _ct)).ReturnsAsync(true);

            Assert.ThrowsAsync<InvalidOperationException>(() => m_service.AddProductAsync(product, _ct));
        }

        [Test]
        public async Task AddProduct_ShouldSave_WhenValid()
        {
            var product = new Product
            {
                Name = "Keyboard",
                Price = 100
            };

            m_store.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), _ct)).ReturnsAsync(false);
            m_store.Setup(x => x.AddProductAsync(It.IsAny<tbl_product>(), _ct))
                   .Returns(Task.CompletedTask);

            var id = await m_service.AddProductAsync(product, _ct);

            Assert.AreNotEqual(Guid.Empty, id);

            m_store.Verify(x => x.AddProductAsync(It.IsAny<tbl_product>(), _ct), Times.Once);
        }

        [Test]
        public async Task DeleteProduct_ShouldCallStoreAndReturnResult()
        {
            var id = Guid.NewGuid();

            m_store.Setup(x => x.DeleteProductAsync(id, _ct)).ReturnsAsync(true);

            var result = await m_service.DeleteProductAsync(id, _ct);

            Assert.IsTrue(result);
            m_store.Verify(x => x.DeleteProductAsync(id, _ct), Times.Once);
        }

        [Test]
        public void GetProduct_ShouldThrow_WhenIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => m_service.GetProductAsync(Guid.Empty, _ct));
        }

        [Test]
        public void GetProduct_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();

            m_store.Setup(x => x.GetProductAsync(id, _ct))
                   .ReturnsAsync((tbl_product)null);

            Assert.ThrowsAsync<InvalidOperationException>(() => m_service.GetProductAsync(id, _ct));
        }

        [Test]
        public async Task GetProduct_ShouldReturnProduct_WhenExists()
        {
            var id = Guid.NewGuid();

            m_store.Setup(x => x.GetProductAsync(id, _ct))
                   .ReturnsAsync(new tbl_product
                   {
                       id = id,
                       name = "Mouse",
                       description = "Gaming mouse",
                       price = 60
                   });

            var result = await m_service.GetProductAsync(id, _ct);

            Assert.AreEqual(id, result.ID);
            Assert.AreEqual("Mouse", result.Name);
        }

        [Test]
        public async Task GetProducts_ShouldReturnList()
        {
            var filter = new ProductFilter();

            m_store.Setup(s => s.GetProductsAsync(filter, _ct)).ReturnsAsync(new List<tbl_product>
        {
            new tbl_product { id = Guid.NewGuid(), name = "A", price = 10 },
            new tbl_product { id = Guid.NewGuid(), name = "B", price = 20 }
        });

            var products = await m_service.GetProductsAsync(filter, _ct);

            Assert.AreEqual(2, products.Count);
        }

        [Test]
        public void UpdateProduct_ShouldThrow_WhenNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => m_service.UpdateProductAsync(null, _ct));
        }

        [Test]
        public void UpdateProduct_ShouldThrow_WhenIdInvalid()
        {
            var product = new Product { Name = "Test", Price = 10 };

            Assert.ThrowsAsync<ArgumentException>(() => m_service.UpdateProductAsync(product, _ct));
        }

        [Test]
        public void UpdateProduct_ShouldThrow_WhenNameEmpty()
        {
            var product = new Product { ID = Guid.NewGuid(), Price = 10 };

            Assert.ThrowsAsync<ArgumentNullException>(() => m_service.UpdateProductAsync(product, _ct));
        }

        [Test]
        public void UpdateProduct_ShouldThrow_WhenPriceInvalid()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "T", Price = 0 };

            Assert.ThrowsAsync<ArgumentException>(() => m_service.UpdateProductAsync(product, _ct));
        }

        [Test]
        public void UpdateProduct_ShouldThrow_WhenProductDoesNotExist()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, Name = "Valid", Price = 50 };

            m_store.Setup(x => x.ExistsAsync(id, _ct)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(() => m_service.UpdateProductAsync(product, _ct));
        }

        [Test]
        public async Task UpdateProduct_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, Name = "Lamp", Price = 40 };

            m_store.Setup(x => x.ExistsAsync(id, _ct)).ReturnsAsync(true);

            m_store.Setup(x => x.UpdateProductAsync(It.IsAny<tbl_product>(), _ct))
                   .ReturnsAsync(true);

            var result = await m_service.UpdateProductAsync(product, _ct);

            Assert.IsTrue(result);

            m_store.Verify(x => x.UpdateProductAsync(It.IsAny<tbl_product>(), _ct), Times.Once);
        }
    }
}