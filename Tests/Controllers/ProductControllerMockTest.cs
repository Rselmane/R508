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
        private Mock<IDataRepository<Product>> _productRepositoryMock;
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
        private List<Product> _productList;
        private List<ProductDTO> _productDTOList;

        /// <summary>
        /// Initialise les mocks et les données communes pour tous les tests
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Création des mocks
            _productRepositoryMock = new Mock<IDataRepository<Product>>();
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

            _productList = new List<Product> { _sampleProduct, _anotherProduct };
            _productDTOList = new List<ProductDTO> { _sampleProductDTO, _anotherProductDTO };
        }

        #region GET

        /// <summary>
        /// Teste la récupération d'un produit existant
        /// </summary>
        [TestMethod]
        public async Task Get_ProductExists_ReturnsOk()
        {
            // Given
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync(_sampleProduct);
            _mapperMock.Setup(m => m.Map<ProductDetailDTO>(_sampleProduct))
                       .Returns(_sampleDetailDTO);

            // When
            var result = await _controller.Get(_sampleProduct.IdProduct);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual(_sampleDetailDTO, ((OkObjectResult)result.Result).Value);
        }

        /// <summary>
        /// Teste la récupération d'un produit inexistant
        /// </summary>
        [TestMethod]
        public async Task Get_ProductDoesNotExist_ReturnsNotFound()
        {
            _sampleProduct.IdProduct = 99;

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync((Product?)null);

            var result = await _controller.Get(_sampleProduct.IdProduct);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Teste la récupération de tous les produits
        /// </summary>
        [TestMethod]
        public async Task GetAll_ReturnsAllProducts()
        {
            _productRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(_productList);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDTO>>(_productList))
                       .Returns(_productDTOList);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            CollectionAssert.AreEqual(_productDTOList.ToList(),
                                      ((OkObjectResult)result.Result).Value as List<ProductDTO>);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Teste la suppression d'un produit existant
        /// </summary>
        [TestMethod]
        public async Task Delete_ProductExists_ReturnsNoContent()
        {
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync(_sampleProduct);
            _productRepositoryMock.Setup(r => r.DeleteAsync(_sampleProduct))
                                  .Returns(Task.CompletedTask);

            var result = await _controller.Delete(_sampleProduct.IdProduct);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _productRepositoryMock.Verify(r => r.DeleteAsync(_sampleProduct), Times.Once);
        }

        /// <summary>
        /// Teste la suppression d'un produit inexistant
        /// </summary>
        [TestMethod]
        public async Task Delete_ProductDoesNotExist_ReturnsNotFound()
        {
            _sampleProduct.IdProduct = 99;
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync((Product?)null);

            var result = await _controller.Delete(_sampleProduct.IdProduct);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region POST

        /// <summary>
        /// Teste la création d'un produit valide
        /// </summary>
        [TestMethod]
        public async Task Create_ValidProduct_ReturnsCreatedAtWhenion()
        {
            ProductAddDTO dto = new ProductAddDTO { Nom = "Chair" };

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(_sampleProduct);
            _productRepositoryMock.Setup(r => r.AddAsync(_sampleProduct)).ReturnsAsync(_sampleProduct);
            _mapperMock.Setup(m => m.Map<ProductDetailDTO>(_sampleProduct)).Returns(_sampleDetailDTO);

            var result = await _controller.Create(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual(_sampleDetailDTO, createdResult.Value);
        }

        /// <summary>
        /// Teste la création d'un produit invalide (modelstate invalide)
        /// </summary>
        [TestMethod]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Nom", "Required");

            var result = await _controller.Create(new ProductAddDTO());

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        #endregion

        #region PUT

        /// <summary>
        /// Teste la mise à jour d'un produit valide
        /// </summary>
        [TestMethod]
        public async Task Update_ValidProduct_ReturnsNoContent()
        {
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync(_sampleProduct);
            _productRepositoryMock.Setup(r => r.UpdateAsync(_sampleProduct, _sampleProduct))
                                  .Returns(Task.CompletedTask);

            var result = await _controller.Update(_sampleProduct.IdProduct, _sampleProduct);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Teste la mise à jour avec un ID qui ne correspond pas
        /// </summary>
        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var result = await _controller.Update(99, _sampleProduct);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Teste la mise à jour d'un produit inexistant
        /// </summary>
        [TestMethod]
        public async Task Update_ProductDoesNotExist_ReturnsNotFound()
        {
            _productRepositoryMock.Setup(r => r.GetByIdAsync(_sampleProduct.IdProduct))
                                  .ReturnsAsync((Product?)null);

            var result = await _controller.Update(_sampleProduct.IdProduct, _sampleProduct);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion
    }
}
