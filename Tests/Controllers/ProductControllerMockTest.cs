using App.Controllers;
using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    [TestClass]
    [TestSubject(typeof(ProductController))]
    [TestCategory("mock")]
    public class ProductControllerMockTest
    {
        // Mocks des dépendances
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<AppDbContext> _contextMock;

        // Instance du contrôleur
        private ProductController _controller;

        // Variables communes utilisées dans les tests
        private Product _sampleProduct;
        private Product _anotherProduct;
        private ProductDTO _sampleProductDTO;
        private ProductDTO _anotherProductDTO;
        private ProductDetailDTO _sampleDetailDTO;
        private ProductAddDTO _sampleAddDTO;
        private List<Product> _productList;
        private List<ProductDTO> _productDTOList;

        [TestInitialize]
        public void Setup()
        {
            // Création des mocks
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<AppDbContext>();

            // Création du contrôleur avec injection des dépendances
            _controller = new ProductController(
                _mapperMock.Object,
                _productRepositoryMock.Object,
                _contextMock.Object
            );

            // Création des produits et DTO réutilisables
            _sampleProduct = new Product { IdProduct = 1, ProductName = "Chair" };
            _anotherProduct = new Product { IdProduct = 2, ProductName = "Table" };

            _sampleProductDTO = new ProductDTO { Id = 1, Name = "Chair" };
            _anotherProductDTO = new ProductDTO { Id = 2, Name = "Table" };

            _sampleDetailDTO = new ProductDetailDTO { Id = 1, Name = "Chair" };
            _sampleAddDTO = new ProductAddDTO { Name = "Chair" };

            _productList = new List<Product> { _sampleProduct, _anotherProduct };
            _productDTOList = new List<ProductDTO> { _sampleProductDTO, _anotherProductDTO };
        }

        #region GET

        [TestMethod]
        public void Get_ProductExists_ReturnsOk()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync(_sampleProduct);
            _mapperMock.Setup(m => m.Map<ProductDetailDTO>(_sampleProduct))
                       .Returns(_sampleDetailDTO);

            // When
            IActionResult result = _controller.Get(_sampleProduct.IdProduct).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_sampleDetailDTO, ((OkObjectResult)result).Value);

            _productRepositoryMock.Verify(r => r.GetByIdAsync(_sampleProduct.IdProduct), Times.Once);
        }

        [TestMethod]
        public void Get_ProductDoesNotExist_ReturnsNotFound()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetByIdAsync(99))
                                  .ReturnsAsync((Product?)null);

            // When
            IActionResult result = _controller.Get(99).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _productRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
        }

        [TestMethod]
        public void GetAll_ReturnsAllProducts()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetAllWithRelationsAsync()).ReturnsAsync(_productList);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDTO>>(_productList))
                       .Returns(_productDTOList);

            // When
            var result = _controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            CollectionAssert.AreEqual(_productDTOList.ToList(),
                                      ((OkObjectResult)result.Result).Value as List<ProductDTO>);

            _productRepositoryMock.Verify(r => r.GetAllWithRelationsAsync(), Times.Once);
        }

        #endregion

        #region DELETE

        [TestMethod]
        public void Delete_ProductExists_ReturnsNoContent()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync(_sampleProduct);
            _productRepositoryMock.Setup(r => r.DeleteAsync(_sampleProduct))
                                  .Returns(Task.CompletedTask);

            // When
            IActionResult result = _controller.Delete(_sampleProduct.IdProduct).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _productRepositoryMock.Verify(r => r.GetByIdAsync(_sampleProduct.IdProduct), Times.Once);
            _productRepositoryMock.Verify(r => r.DeleteAsync(_sampleProduct), Times.Once);
        }

        [TestMethod]
        public void Delete_ProductDoesNotExist_ReturnsNotFound()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetByIdAsync(99))
                                  .ReturnsAsync((Product?)null);

            // When
            IActionResult result = _controller.Delete(99).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _productRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            _productRepositoryMock.Verify(r => r.DeleteAsync(_sampleProduct), Times.Never);
        }

        #endregion

        #region POST

        [TestMethod]
        public void Create_ValidProduct_ReturnsCreatedAtAction()
        {
            // Given
            ProductAddDTO dto = new ProductAddDTO { Name = "Chair" };

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(_sampleProduct);
            _productRepositoryMock.Setup(r => r.AddAsync(_sampleProduct)).ReturnsAsync(_sampleProduct);
            _mapperMock.Setup(m => m.Map<ProductDetailDTO>(_sampleProduct)).Returns(_sampleDetailDTO);

            // When
            IActionResult result = _controller.Create(dto).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            Assert.AreEqual(_sampleDetailDTO, createdResult.Value);

            _productRepositoryMock.Verify(r => r.AddAsync(_sampleProduct), Times.Once);
        }

        #endregion

        #region PUT

        [TestMethod]
        public void Update_ValidProduct_ReturnsNoContent()
        {
            // Given
            Product updatedProduct = new Product { IdProduct = _sampleProduct.IdProduct, ProductName = "Updated Chair" };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync(_sampleProduct);
            _mapperMock.Setup(m => m.Map<Product>(_sampleAddDTO))
                       .Returns(updatedProduct);
            _productRepositoryMock.Setup(r => r.UpdateAsync(_sampleProduct, updatedProduct))
                                  .Returns(Task.CompletedTask);

            // When
            IActionResult result = _controller.Update(_sampleProduct.IdProduct, _sampleAddDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _productRepositoryMock.Verify(r => r.GetByIdAsync(_sampleProduct.IdProduct), Times.Once);
            _productRepositoryMock.Verify(r => r.UpdateAsync(_sampleProduct, updatedProduct), Times.Once);
        }

        [TestMethod]
        public void Update_ProductDoesNotExist_ReturnsNotFound()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync((Product?)null);

            // When
            IActionResult result = _controller.Update(_sampleProduct.IdProduct, _sampleAddDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _productRepositoryMock.Verify(r => r.GetByIdAsync(_sampleProduct.IdProduct), Times.Once);
            _productRepositoryMock.Verify(r => r.UpdateAsync(_sampleProduct, _sampleProduct), Times.Never);
        }

        #endregion
    }
}