using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Api.Controllers;
using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using Moq;

namespace MiniDashboard.Api.Test
{
    public class ProductControllerTests
    {
        private Mock<IProductService> m_service;
        
        private ProductController m_controller;

        private CancellationToken m_ct;

        [SetUp]
        public void Setup()
        {
            m_service = new Mock<IProductService>(MockBehavior.Strict);
            m_controller = new ProductController(m_service.Object);
            m_ct = CancellationToken.None;
        }

        [Test]
        public async Task GetAll_ShouldReturnOkAndList()
        {
            var expected = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "A", Price = 10 }
            };

            m_service.Setup(s => s.GetProductsAsync(
                    It.IsAny<ProductFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var action = await m_controller.GetAll(m_ct);
            var result = action.Result as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var products = result.Value as List<Product>;
            Assert.That(products, Is.EqualTo(expected));
        }

        [Test]
        public async Task Get_ShouldReturnOk_WhenProductExists()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, Name = "Mouse", Price = 20 };

            m_service.Setup(s => s.GetProductAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var action = await m_controller.Get(id, m_ct);
            var result = action.Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(product));
        }

        [Test]
        public async Task Get_ShouldReturnNotFound_WhenNoProduct()
        {
            var id = Guid.NewGuid();

            m_service.Setup(s => s.GetProductAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Product not found"));

            var action = await m_controller.Get(id, m_ct);
            var result = action.Result as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.EqualTo("Product not found"));
        }

        [Test]
        public async Task Search_ShouldCallServiceWithCorrectFilter()
        {
            List<Product> expected = new();
            ProductFilter captured = null;

            m_service.Setup(s => s.GetProductsAsync(
                    It.IsAny<ProductFilter>(),
                    It.IsAny<CancellationToken>()))
                .Callback<ProductFilter, CancellationToken>((f, _) => captured = f)
                .ReturnsAsync(expected);

            var action = await m_controller.Search("mouse", 10, 50, StringFilterType.StartsWith, m_ct);

            var result = action.Result as OkObjectResult;

            Assert.NotNull(result);

            Assert.NotNull(captured);
            Assert.That(captured.Name.Value, Is.EqualTo("mouse"));
            Assert.That(captured.Name.FilterType, Is.EqualTo(StringFilterType.StartsWith));
            Assert.That(captured.MinPrice, Is.EqualTo(10));
            Assert.That(captured.MaxPrice, Is.EqualTo(50));
        }

        [Test]
        public async Task Add_ShouldReturnCreatedId()
        {
            var id = Guid.NewGuid();
            var product = new Product { Name = "Laptop", Price = 1000 };

            m_service.Setup(s => s.AddProductAsync(product, It.IsAny<CancellationToken>()))
                .ReturnsAsync(id);

            var action = await m_controller.Add(product, m_ct);
            var result = action.Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(id));
        }

        [Test]
        public async Task Add_ShouldReturnBadRequest_OnArgumentException()
        {
            var product = new Product();

            m_service.Setup(s => s.AddProductAsync(product, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("Price invalid"));

            var action = await m_controller.Add(product, m_ct);
            var result = action.Result as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Price invalid"));
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "X", Price = 20 };

            var result = await m_controller.Update(Guid.NewGuid(), product, m_ct) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Product ID in URI and body must match."));
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, Name = "Chair", Price = 50 };

            m_service.Setup(s => s.UpdateProductAsync(product, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await m_controller.Update(id, product, m_ct) as OkResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Update_ShouldReturnNotFound_WhenNotUpdated()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, Name = "Chair", Price = 50 };

            m_service.Setup(s => s.UpdateProductAsync(product, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await m_controller.Update(id, product, m_ct) as NotFoundResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Delete_ShouldReturnOk_WhenDeleted()
        {
            var id = Guid.NewGuid();

            m_service.Setup(s => s.DeleteProductAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await m_controller.Delete(id, m_ct) as OkResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenNotDeleted()
        {
            var id = Guid.NewGuid();

            m_service.Setup(s => s.DeleteProductAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await m_controller.Delete(id, m_ct) as NotFoundResult;

            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
    }
}
