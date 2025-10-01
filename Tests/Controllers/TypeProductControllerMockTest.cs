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
        private List<TypeProduct> _typeProductList;
        private List<TypeProductDTO> _typeProductDTOList;

        /// <summary>
        /// Initialise les mocks et les données communes pour tous les tests
        /// </summary>
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

            // Création des marques et DTO réutilisables
            _sampleTypeProduct = new TypeProduct { IdTypeProduct = 1, TypeProductName = "IKA" };
            _anotherTypeProduct = new TypeProduct { IdTypeProduct = 2, TypeProductName = "Poltrone Et Sofa" };

            _sampleTypeProductDTO = new TypeProductDTO { Id = 1, Name = "IKA" };
            _anotherTypeProductDTO = new TypeProductDTO { Id = 2, Name = "Poltrone Et Sofa" };

            _typeProductList = new List<TypeProduct> { _sampleTypeProduct, _anotherTypeProduct };
            _typeProductDTOList = new List<TypeProductDTO> { _sampleTypeProductDTO, _anotherTypeProductDTO };
        }

        #region GET

        /// <summary>
        /// Teste la récupération d'une marque existant
        /// </summary>
        [TestMethod]
        public async Task Get_TypeProductExists_ReturnsOk()
        {
            // Given
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync(_sampleTypeProduct);
            _mapperMock.Setup(m => m.Map<TypeProductDTO>(_sampleTypeProduct))
                       .Returns(_sampleTypeProductDTO);

            // When
            var result = await _controller.Get(_sampleTypeProduct.IdTypeProduct);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual(_sampleTypeProductDTO, ((OkObjectResult)result.Result).Value);
        }

        /// <summary>
        /// Teste la récupération d'un marque inexistant
        /// </summary>
        [TestMethod]
        public async Task Get_TypeProductDoesNotExist_ReturnsNotFound()
        {
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync((TypeProduct?)null);

            var result = await _controller.Get(99);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Teste la récupération de tous les marques
        /// </summary>
        [TestMethod]
        public async Task GetAll_ReturnsAllTypeProducts()
        {
            _typeProductRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(_typeProductList);
            _mapperMock.Setup(m => m.Map<IEnumerable<TypeProductDTO>>(_typeProductList))
                       .Returns(_typeProductDTOList);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            CollectionAssert.AreEqual(_typeProductDTOList.ToList(),
                                      ((OkObjectResult)result.Result).Value as List<TypeProductDTO>);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Teste la suppression d'une marque existant
        /// </summary>
        [TestMethod]
        public async Task Delete_TypeProductExists_ReturnsNoContent()
        {
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync(_sampleTypeProduct);
            _typeProductRepositoryMock.Setup(r => r.DeleteAsync(_sampleTypeProduct))
                                  .Returns(Task.CompletedTask);

            var result = await _controller.Delete(_sampleTypeProduct.IdTypeProduct);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _typeProductRepositoryMock.Verify(r => r.DeleteAsync(_sampleTypeProduct), Times.Once);
        }

        /// <summary>
        /// Teste la suppression d'un produit inexistant
        /// </summary>
        [TestMethod]
        public async Task Delete_TypeProductDoesNotExist_ReturnsNotFound()
        {
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync((TypeProduct?)null);

            var result = await _controller.Delete(99);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region POST

        /// <summary>
        /// Teste la création d'une marque valide
        /// </summary>
        [TestMethod]
        public async Task Create_ValidTypeProduct_ReturnsCreatedAtWhenion()
        {
            var dto = new TypeProductDTO { Name = "IKA" };

            _mapperMock.Setup(m => m.Map<TypeProduct>(dto)).Returns(_sampleTypeProduct);
            _typeProductRepositoryMock.Setup(r => r.AddAsync(_sampleTypeProduct)).ReturnsAsync(_sampleTypeProduct);
            _mapperMock.Setup(m => m.Map<TypeProductDTO>(_sampleTypeProduct)).Returns(_sampleTypeProductDTO);

            var result = await _controller.Create(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual(_sampleTypeProductDTO, createdResult.Value);
        }

        /// <summary>
        /// Teste la création d'une marque invalide (modelstate invalide)
        /// </summary>
        [TestMethod]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Create(new TypeProductDTO());

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        #endregion

        #region PUT

        /// <summary>
        /// Teste la mise à jour d'une marque valide
        /// </summary>
        [TestMethod]
        public async Task Update_ValidTypeProduct_ReturnsNoContent()
        {
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync(_sampleTypeProduct);
            _typeProductRepositoryMock.Setup(r => r.UpdateAsync(_sampleTypeProduct, _sampleTypeProduct))
                                  .Returns(Task.CompletedTask);

            var result = await _controller.Update(_sampleTypeProduct.IdTypeProduct, _sampleTypeProduct);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Teste la mise à jour avec un ID qui ne correspond pas
        /// </summary>
        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var result = await _controller.Update(99, _sampleTypeProduct);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Teste la mise à jour d'une marque inexistant
        /// </summary>
        [TestMethod]
        public async Task Update_TypeProductDoesNotExist_ReturnsNotFound()
        {
            _typeProductRepositoryMock.Setup(r => r.GetByIdAsync(_sampleTypeProduct.IdTypeProduct))
                                  .ReturnsAsync((TypeProduct?)null);

            var result = await _controller.Update(_sampleTypeProduct.IdTypeProduct, _sampleTypeProduct);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion
    }
}
