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
    [TestSubject(typeof(TypeProductController))]
    [TestCategory("mock")]
    public class TypeProductControllerMockTest
    {
        // Mocks des dépendances
        private Mock<IDataRepository<TypeProduct>> _typeProductRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<AppDbContext> _contextMock;

        // Instance du contrôleur
        private TypeProductController _controller;

        // Variables communes utilisées dans les tests
        private TypeProduct _sampleTypeProduct;
        private TypeProduct _anotherTypeProduct;
        private TypeProductDTO _sampleTypeProductDTO;
        private TypeProductDTO _anotherTypeProductDTO;
        private TypeProductUpdateDTO _sampleTypeProductUpdateDTO;
        private List<TypeProduct> _typeProductList;
        private List<TypeProductDTO> _typeProductDTOList;

        [TestInitialize]
        public void Setup()
        {
            // Création des mocks
            _typeProductRepositoryMock = new Mock<IDataRepository<TypeProduct>>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<AppDbContext>();

            // Création du contrôleur avec injection des dépendances
            _controller = new TypeProductController(
                _mapperMock.Object,
                _typeProductRepositoryMock.Object,
                _contextMock.Object
            );

            // Création des types de produits et DTO réutilisables
            _sampleTypeProduct = new TypeProduct { IdTypeProduct = 1, TypeProductName = "Wood Table" };
            _anotherTypeProduct = new TypeProduct { IdTypeProduct = 2, TypeProductName = "Plastic Chair" };

            _sampleTypeProductDTO = new TypeProductDTO { Id = 1, Name = "Wood Table" };
            _anotherTypeProductDTO = new TypeProductDTO { Id = 2, Name = "Plastic Chair" };
            _sampleTypeProductUpdateDTO = new TypeProductUpdateDTO { Name = "Metal Table" };

            _typeProductList = new List<TypeProduct> { _sampleTypeProduct, _anotherTypeProduct };
            _typeProductDTOList = new List<TypeProductDTO> { _sampleTypeProductDTO, _anotherTypeProductDTO };
        }

        #region GET

        [TestMethod]
        public void Get_TypeProductExists_ReturnsOk()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync(_sampleTypeProduct);
            _mapperMock.Setup(m => m.Map<TypeProductDTO>(_sampleTypeProduct))
                       .Returns(_sampleTypeProductDTO);

            // When
            IActionResult result = _controller.Get(_sampleTypeProduct.IdTypeProduct).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_sampleTypeProductDTO, ((OkObjectResult)result).Value);

            _typeProductRepositoryMock.Verify(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct), Times.Once);
        }

        [TestMethod]
        public void Get_TypeProductDoesNotExist_ReturnsNotFound()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(99))
                                          .ReturnsAsync((TypeProduct?)null);

            // When
            IActionResult result = _controller.Get(99).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _typeProductRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
        }

        [TestMethod]
        public void GetAll_ReturnsAllTypeProducts()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(_typeProductList);
            _mapperMock.Setup(m => m.Map<IEnumerable<TypeProductDTO>>(_typeProductList))
                       .Returns(_typeProductDTOList);

            // When
            var result = _controller.GetAll().GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            CollectionAssert.AreEqual(_typeProductDTOList.ToList(),
                                                  ((OkObjectResult)result.Result).Value as List<TypeProductDTO>);

            _typeProductRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        #endregion

        #region DELETE

        [TestMethod]
        public void Delete_TypeProductExists_ReturnsNoContent()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync(_sampleTypeProduct);
            _typeProductRepositoryMock.Setup(r => r.DeleteAsync(_sampleTypeProduct))
                                  .Returns(Task.CompletedTask);

            // When
            var result = _controller.Delete(_sampleTypeProduct.IdTypeProduct).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _typeProductRepositoryMock.Verify(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct), Times.Once);
            _typeProductRepositoryMock.Verify(r => r.DeleteAsync(_sampleTypeProduct), Times.Once);
        }

        [TestMethod]
        public void Delete_TypeProductDoesNotExist_ReturnsNotFound()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(99))
                                  .ReturnsAsync((TypeProduct?)null);

            // When
            var result = _controller.Delete(99).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _typeProductRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            _typeProductRepositoryMock.Verify(r => r.DeleteAsync(_sampleTypeProduct), Times.Never);
        }

        #endregion

        #region POST

        [TestMethod]
        public void Create_ValidTypeProduct_ReturnsCreatedAtAction()
        {
            // Given
            TypeProductDTO dto = new TypeProductDTO { Name = "IKA" };

            _mapperMock.Setup(m => m.Map<TypeProduct>(dto)).Returns(_sampleTypeProduct);
            _typeProductRepositoryMock.Setup(r => r.AddAsync(_sampleTypeProduct)).ReturnsAsync(_sampleTypeProduct);
            _mapperMock.Setup(m => m.Map<TypeProductDTO>(_sampleTypeProduct)).Returns(_sampleTypeProductDTO);

            // When
            IActionResult result = _controller.Create(dto).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            Assert.AreEqual(_sampleTypeProductDTO, createdResult.Value);

            _typeProductRepositoryMock.Verify(r => r.AddAsync(_sampleTypeProduct), Times.Once);
        }

        #endregion

        #region PUT

        [TestMethod]
        public void Update_ValidTypeProduct_ReturnsNoContent()
        {
            // Given
            TypeProduct updatedTypeProduct = new TypeProduct { IdTypeProduct = _sampleTypeProduct.IdTypeProduct, TypeProductName = "Metal Table" };

            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync(_sampleTypeProduct);
            _mapperMock.Setup(m => m.Map<TypeProduct>(_sampleTypeProductUpdateDTO))
                       .Returns(updatedTypeProduct);
            _typeProductRepositoryMock.Setup(r => r.UpdateAsync(_sampleTypeProduct, updatedTypeProduct))
                                  .Returns(Task.CompletedTask);

            // When
            IActionResult result = _controller.Update(_sampleTypeProduct.IdTypeProduct, _sampleTypeProductUpdateDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _typeProductRepositoryMock.Verify(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct), Times.Once);
            _typeProductRepositoryMock.Verify(r => r.UpdateAsync(_sampleTypeProduct, updatedTypeProduct), Times.Once);
        }

        [TestMethod]
        public void Update_TypeProductDoesNotExist_ReturnsNotFound()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync((TypeProduct?)null);

            // When
               IActionResult result = _controller.Update(_sampleTypeProduct.IdTypeProduct, _sampleTypeProductUpdateDTO).GetAwaiter().GetResult();

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _typeProductRepositoryMock.Verify(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct), Times.Once);
            _typeProductRepositoryMock.Verify(r => r.UpdateAsync(_sampleTypeProduct, _sampleTypeProduct), Times.Never);
        }

        #endregion
    }
}