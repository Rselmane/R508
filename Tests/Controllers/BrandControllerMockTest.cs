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
    [TestSubject(typeof(BrandController))]
    [TestCategory("mock")]
    public class BrandControllerMockTest
    {
        // Mocks des dépendances
        private Mock<IDataRepository<Brand>> _brandRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<AppDbContext> _contextMock;

        // Instance du contrôleur
        private BrandController _controller;

        // Variables communes utilisées dans les tests
        private Brand _sampleBrand;
        private Brand _anotherBrand;
        private BrandDTO _sampleBrandDTO;
        private BrandDTO _anotherBrandDTO;
        private List<Brand> _brandList;
        private List<BrandDTO> _brandDTOList;

        /// <summary>
        /// Initialise les mocks et les données communes pour tous les tests
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Création des mocks
            _brandRepositoryMock = new Mock<IDataRepository<Brand>>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<AppDbContext>();

            // Création du contrôleur avec injection des dépendances
            _controller = new BrandController(
                _mapperMock.Object,
                _brandRepositoryMock.Object,
                _contextMock.Object
            );

            // Création des marques et DTO réutilisables
            _sampleBrand = new Brand { IdBrand = 1, BrandName = "IKA" };
            _anotherBrand = new Brand { IdBrand = 2, BrandName = "Poltrone Et Sofa" };

            _sampleBrandDTO = new BrandDTO { Id = 1, Name = "IKA" };
            _anotherBrandDTO = new BrandDTO { Id = 2, Name = "Poltrone Et Sofa" };

            _brandList = new List<Brand> { _sampleBrand, _anotherBrand };
            _brandDTOList = new List<BrandDTO> { _sampleBrandDTO, _anotherBrandDTO };
        }

        #region GET

        /// <summary>
        /// Teste la récupération d'une marque existant
        /// </summary>
        [TestMethod]
        public async Task Get_BrandExists_ReturnsOk()
        {
            // Given
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync(_sampleBrand);
            _mapperMock.Setup(m => m.Map<BrandDTO>(_sampleBrand))
                       .Returns(_sampleBrandDTO);

            // When
            var result = await _controller.Get(_sampleBrand.IdBrand);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual(_sampleBrandDTO, ((OkObjectResult)result.Result).Value);
        }

        /// <summary>
        /// Teste la récupération d'un marque inexistant
        /// </summary>
        [TestMethod]
        public async Task Get_BrandDoesNotExist_ReturnsNotFound()
        {
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync((Brand?)null);

            var result = await _controller.Get(99);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Teste la récupération de tous les marques
        /// </summary>
        [TestMethod]
        public async Task GetAll_ReturnsAllBrands()
        {
            _brandRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(_brandList);
            _mapperMock.Setup(m => m.Map<IEnumerable<BrandDTO>>(_brandList))
                       .Returns(_brandDTOList);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            CollectionAssert.AreEqual(_brandDTOList.ToList(),
                                      ((OkObjectResult)result.Result).Value as List<BrandDTO>);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Teste la suppression d'une marque existant
        /// </summary>
        [TestMethod]
        public async Task Delete_BrandExists_ReturnsNoContent()
        {
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync(_sampleBrand);
            _brandRepositoryMock.Setup(r => r.DeleteAsync(_sampleBrand))
                                  .Returns(Task.CompletedTask);

            var result = await _controller.Delete(_sampleBrand.IdBrand);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _brandRepositoryMock.Verify(r => r.DeleteAsync(_sampleBrand), Times.Once);
        }

        /// <summary>
        /// Teste la suppression d'un produit inexistant
        /// </summary>
        [TestMethod]
        public async Task Delete_BrandDoesNotExist_ReturnsNotFound()
        {
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync((Brand?)null);

            var result = await _controller.Delete(99);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region POST

        /// <summary>
        /// Teste la création d'une marque valide
        /// </summary>
        [TestMethod]
        public async Task Create_ValidBrand_ReturnsCreatedAtWhenion()
        {
            var dto = new BrandDTO { Name = "IKA" };

            _mapperMock.Setup(m => m.Map<Brand>(dto)).Returns(_sampleBrand);
            _brandRepositoryMock.Setup(r => r.AddAsync(_sampleBrand)).ReturnsAsync(_sampleBrand);
            _mapperMock.Setup(m => m.Map<BrandDTO>(_sampleBrand)).Returns(_sampleBrandDTO);

            var result = await _controller.Create(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual(_sampleBrandDTO, createdResult.Value);
        }

        /// <summary>
        /// Teste la création d'une marque invalide (modelstate invalide)
        /// </summary>
        [TestMethod]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Create(new BrandDTO());

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        #endregion

        #region PUT

        /// <summary>
        /// Teste la mise à jour d'une marque valide
        /// </summary>
        [TestMethod]
        public async Task Update_ValidBrand_ReturnsNoContent()
        {
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync(_sampleBrand);
            _brandRepositoryMock.Setup(r => r.UpdateAsync(_sampleBrand, _sampleBrand))
                                  .Returns(Task.CompletedTask);

            var result = await _controller.Update(_sampleBrand.IdBrand, _sampleBrand);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Teste la mise à jour avec un ID qui ne correspond pas
        /// </summary>
        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var result = await _controller.Update(99, _sampleBrand);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Teste la mise à jour d'une marque inexistant
        /// </summary>
        [TestMethod]
        public async Task Update_BrandDoesNotExist_ReturnsNotFound()
        {
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync((Brand?)null);

            var result = await _controller.Update(_sampleBrand.IdBrand, _sampleBrand);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion
    }
}
