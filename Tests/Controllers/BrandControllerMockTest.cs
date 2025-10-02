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
        private BrandUpdateDTO _sampleBrandUpdateDTO;
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
            _sampleBrandUpdateDTO = new BrandUpdateDTO { Name = "IKA Updated" };

            _brandList = new List<Brand> { _sampleBrand, _anotherBrand };
            _brandDTOList = new List<BrandDTO> { _sampleBrandDTO, _anotherBrandDTO };
        }

        #region GET

        /// <summary>
        /// Teste la récupération d'une marque existante
        /// </summary>
        [TestMethod]
        public void Get_BrandExists_ReturnsOk()
        {
            // Given : Une marque enregistrée
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync(_sampleBrand);
            _mapperMock.Setup(m => m.Map<BrandDTO>(_sampleBrand))
                       .Returns(_sampleBrandDTO);

            // When : On appelle la méthode GET de l'API pour récupérer la marque
            var result = _controller.Get(_sampleBrand.IdBrand).GetAwaiter().GetResult();

            // Then : On récupère la marque et le code de retour est 200
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual(_sampleBrandDTO, ((OkObjectResult)result.Result).Value);

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(_sampleBrand.IdBrand), Times.Once);
        }

        /// <summary>
        /// Teste la récupération d'une marque inexistante
        /// </summary>
        [TestMethod]
        public void Get_BrandDoesNotExist_ReturnsNotFound()
        {
            // Given : Pas de marque trouvée par le manager
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(99))
                                  .ReturnsAsync((Brand?)null);

            // When : On appelle la méthode GET de l'API pour récupérer une marque inexistante
            var result = _controller.Get(99).GetAwaiter().GetResult();

            // Then : On ne renvoie rien et on renvoie NOT_FOUND (404)
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
        }

        /// <summary>
        /// Teste la récupération de toutes les marques
        /// </summary>
        [TestMethod]
        public void GetAll_ReturnsAllBrands()
        {
            // Given : Des marques enregistrées
            _brandRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(_brandList);
            _mapperMock.Setup(m => m.Map<IEnumerable<BrandDTO>>(_brandList))
                       .Returns(_brandDTOList);

            // When : On souhaite récupérer toutes les marques
            var result = _controller.GetAll().GetAwaiter().GetResult();

            // Then : Toutes les marques sont récupérées
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            CollectionAssert.AreEqual(_brandDTOList.ToList(),
                                      ((OkObjectResult)result.Result).Value as List<BrandDTO>);

            _brandRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Teste la suppression d'une marque existante
        /// </summary>
        [TestMethod]
        public void Delete_BrandExists_ReturnsNoContent()
        {
            // Given : Une marque enregistrée
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync(_sampleBrand);
            _brandRepositoryMock.Setup(r => r.DeleteAsync(_sampleBrand))
                                  .Returns(Task.CompletedTask);

            // When : On souhaite supprimer une marque depuis l'API
            var result = _controller.Delete(_sampleBrand.IdBrand).GetAwaiter().GetResult();

            // Then : La marque a bien été supprimée et le code HTTP est NO_CONTENT (204)
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(_sampleBrand.IdBrand), Times.Once);
            _brandRepositoryMock.Verify(r => r.DeleteAsync(_sampleBrand), Times.Once);
        }

        /// <summary>
        /// Teste la suppression d'une marque inexistante
        /// </summary>
        [TestMethod]
        public void Delete_BrandDoesNotExist_ReturnsNotFound()
        {
            // Given : Une marque qui n'est pas enregistrée
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(99))
                                  .ReturnsAsync((Brand?)null);

            // When : On souhaite supprimer une marque inexistante depuis l'API
            var result = _controller.Delete(99).GetAwaiter().GetResult();

            // Then : L'API renvoie NOT_FOUND (404)
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            _brandRepositoryMock.Verify(r => r.DeleteAsync(_sampleBrand), Times.Never);
        }

        #endregion

        #region POST

        /// <summary>
        /// Teste la création d'une marque valide
        /// </summary>
        [TestMethod]
        public void Create_ValidBrand_ReturnsCreatedAtAction()
        {
            // Given : Une marque à enregistrer
            BrandUpdateDTO dto = new BrandUpdateDTO { Name = "IKA" };

            _mapperMock.Setup(m => m.Map<Brand>(dto)).Returns(_sampleBrand);
            _brandRepositoryMock.Setup(r => r.AddAsync(_sampleBrand)).ReturnsAsync(_sampleBrand);
            _mapperMock.Setup(m => m.Map<BrandDTO>(_sampleBrand)).Returns(_sampleBrandDTO);

            // When : On appelle la méthode POST de l'API pour enregistrer la marque
            var result = _controller.Create(dto).GetAwaiter().GetResult();

            // Then : La marque est bien enregistrée et le code renvoyé est CREATED (201)
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            Assert.AreEqual(_sampleBrandDTO, createdResult.Value);

            _brandRepositoryMock.Verify(r => r.AddAsync(_sampleBrand), Times.Once);
        }

        /// <summary>
        /// Teste la création d'une marque invalide (modelstate invalide)
        /// </summary>
        [TestMethod]
        public void Create_InvalidModel_ReturnsBadRequest()
        {
            // Given : Un ModelState invalide
            _controller.ModelState.AddModelError("Name", "Required");

            // When : On appelle la méthode POST avec un DTO invalide
            var result = _controller.Create(new BrandUpdateDTO()).GetAwaiter().GetResult();

            // Then : L'API renvoie BAD_REQUEST (400)
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            _brandRepositoryMock.Verify(r => r.AddAsync(_sampleBrand), Times.Never);
        }

        #endregion

        #region PUT

        /// <summary>
        /// Teste la mise à jour d'une marque valide
        /// </summary>
        [TestMethod]
        public void Update_ValidBrand_ReturnsNoContent()
        {
            // Given : Une marque à mettre à jour
            var updatedBrand = new Brand { IdBrand = _sampleBrand.IdBrand, BrandName = "IKA Updated" };

            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync(_sampleBrand);
            _mapperMock.Setup(m => m.Map<Brand>(_sampleBrandUpdateDTO))
                       .Returns(updatedBrand);
            _brandRepositoryMock.Setup(r => r.UpdateAsync(_sampleBrand, updatedBrand))
                                  .Returns(Task.CompletedTask);

            // When : On appelle la méthode PUT du controller pour mettre à jour la marque
            var result = _controller.Update(_sampleBrand.IdBrand, _sampleBrandUpdateDTO).GetAwaiter().GetResult();

            // Then : On vérifie que la marque a bien été modifiée et que le code renvoyé est NO_CONTENT (204)
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(_sampleBrand.IdBrand), Times.Once);
            _brandRepositoryMock.Verify(r => r.UpdateAsync(_sampleBrand, updatedBrand), Times.Once);
        }

        /// <summary>
        /// Teste la mise à jour d'une marque inexistante
        /// </summary>
        [TestMethod]
        public void Update_BrandDoesNotExist_ReturnsNotFound()
        {
            // Given : Une marque à mettre à jour qui n'est pas enregistrée
            _brandRepositoryMock.Setup(r => r.GetByIdAsync(_sampleBrand.IdBrand))
                                  .ReturnsAsync((Brand?)null);

            // When : On appelle la méthode PUT du controller pour mettre à jour une marque qui n'est pas enregistrée
            var result = _controller.Update(_sampleBrand.IdBrand, _sampleBrandUpdateDTO).GetAwaiter().GetResult();

            // Then : On vérifie que l'API renvoie un code NOT_FOUND (404)
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(_sampleBrand.IdBrand), Times.Once);
            _brandRepositoryMock.Verify(r => r.UpdateAsync(_sampleBrand, _sampleBrand), Times.Never);
        }

        /// <summary>
        /// Teste la mise à jour avec un ModelState invalide
        /// </summary>
        [TestMethod]
        public void Update_InvalidModelState_ReturnsBadRequest()
        {
            // Given : Un ModelState invalide
            _controller.ModelState.AddModelError("Name", "Required");

            // When : On appelle la méthode PUT avec un DTO invalide
            var result = _controller.Update(_sampleBrand.IdBrand, _sampleBrandUpdateDTO).GetAwaiter().GetResult();

            // Then : On vérifie que l'API renvoie un code BAD_REQUEST (400)
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(_sampleBrand.IdBrand), Times.Never);
            _brandRepositoryMock.Verify(r => r.UpdateAsync(_sampleBrand, _sampleBrand), Times.Never);
        }

        #endregion
    }
}