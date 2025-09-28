using App.Controllers;
using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(ProductController))]
[TestCategory("mock")]
public class ProductControllerMockTest
{
    private readonly ProductController _productController;
    private readonly Mock<IDataRepository<Product>> _produitManager;
    private readonly Mock<IMapper<Product, ProductDTO>> _produitMapperDTO;
    private readonly Mock<IMapper<Product, ProductDetailDTO>> _produitDetailMapper;
    private readonly Mock<IMapper<Product, ProductAddDTO>> _productAddMapper;
    private readonly Mock<AppDbContext> _context;

    public ProductControllerMockTest()
    {
        _produitManager = new Mock<IDataRepository<Product>>();
        _produitMapperDTO = new Mock<IMapper<Product, ProductDTO>>();
        _produitDetailMapper = new Mock<IMapper<Product, ProductDetailDTO>>();
        _productAddMapper = new Mock<IMapper<Product, ProductAddDTO>>();
        _context = new Mock<AppDbContext>();

        _productController = new ProductController(
            _produitMapperDTO.Object,
            _produitDetailMapper.Object,
            _productAddMapper.Object,
            _produitManager.Object,
            _context.Object);
    }

    [TestMethod]
    public void ShouldGetProduct()
    {
        // Given : Un produit en enregistré
        Product produitInDb = new()
        {
            IdProduct = 30,
            ProductName = "Chaise",
            Description = "Une superbe chaise",
            PhotoName = "Une superbe chaise bleu",
            PhotoUri = "https://ikea.fr/chaise.jpg"
        };

        ProductDetailDTO expectedDto = new()
        {
            Id = 30,
            Name = "Chaise",
            Description = "Une superbe chaise"
        };

        _produitManager
            .Setup(manager => manager.GetByIdAsync(produitInDb.IdProduct))
            .ReturnsAsync(produitInDb);

        _produitDetailMapper
            .Setup(mapper => mapper.ToDTO(produitInDb))
            .Returns(expectedDto);

        // When : On appelle la méthode GET de l'API pour récupérer le produit
        ActionResult<ProductDetailDTO> action = _productController.Get(produitInDb.IdProduct).GetAwaiter().GetResult();

        // Then : On récupère le produit et le code de retour est 200
        _produitManager.Verify(manager => manager.GetByIdAsync(produitInDb.IdProduct), Times.Once);
        _produitDetailMapper.Verify(mapper => mapper.ToDTO(produitInDb), Times.Once);

        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));

        var okResult = (OkObjectResult)action.Result;
        Assert.AreEqual(expectedDto, okResult.Value);
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // Given : Un produit enregistré
        Product produitInDb = new()
        {
            IdProduct = 20,
            ProductName = "Chaise",
            Description = "Une superbe chaise",
            PhotoName = "Une superbe chaise bleu",
            PhotoUri = "https://ikea.fr/chaise.jpg"
        };

        _produitManager
            .Setup(manager => manager.GetByIdAsync(produitInDb.IdProduct))
            .ReturnsAsync(produitInDb);

        _produitManager
            .Setup(manager => manager.DeleteAsync(produitInDb))
            .Returns(Task.CompletedTask);

        // When : On souhaite supprimer un produit depuis l'API
        IActionResult action = _productController.Delete(produitInDb.IdProduct).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        _produitManager.Verify(manager => manager.GetByIdAsync(produitInDb.IdProduct), Times.Once);
        _produitManager.Verify(manager => manager.DeleteAsync(produitInDb), Times.Once);
    }

    [TestMethod]
    public void ShouldNotDeleteProductBecauseProductDoesNotExist()
    {
        // Given : Aucun produit trouvé
        int produitId = 30;

        _produitManager
            .Setup(manager => manager.GetByIdAsync(produitId))
            .ReturnsAsync((Product?)null);

        // When : On souhaite supprimer un produit depuis l'API
        IActionResult action = _productController.Delete(produitId).GetAwaiter().GetResult();

        // Then : L'API renvoie NOT_FOUND (404)
        _produitManager.Verify(manager => manager.GetByIdAsync(produitId), Times.Once);

        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        // Given : Des produits enregistrés
        IEnumerable<Product> productInDb = [
            new()
            {
                IdProduct = 1,
                ProductName = "Chaise",
                Description = "Une superbe chaise",
                PhotoName = "Une superbe chaise bleu",
                PhotoUri = "https://ikea.fr/chaise.jpg"
            },
            new()
            {
                IdProduct = 2,
                ProductName = "Armoir",
                Description = "Une superbe armoire",
                PhotoName = "Une superbe armoire jaune",
                PhotoUri = "https://ikea.fr/armoire-jaune.jpg"
            }
        ];

        IEnumerable<ProductDTO> expectedDtos = [
            new() { Id = 1, Name = "Chaise" },
            new() { Id = 2, Name = "Armoir" }
        ];

        _produitManager
            .Setup(manager => manager.GetAllAsync())
            .ReturnsAsync(productInDb);

        _produitMapperDTO
            .Setup(mapper => mapper.ToDTOs(productInDb))
            .Returns(expectedDtos);

        // When : On souhaite récupérer tous les produits
        var action = _productController.GetAll().GetAwaiter().GetResult();

        // Then : Tous les produits sont récupérés
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));

        var okResult = (OkObjectResult)action.Result;
        Assert.IsTrue(expectedDtos.SequenceEqual((IEnumerable<ProductDTO>)okResult.Value));

        _produitManager.Verify(manager => manager.GetAllAsync(), Times.Once);
        _produitMapperDTO.Verify(mapper => mapper.ToDTOs(productInDb), Times.Once);
    }

    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // Given : Pas de produit trouvé par le manager
        int produitId = 30;

        _produitManager
            .Setup(manager => manager.GetByIdAsync(produitId))
            .ReturnsAsync((Product?)null);

        // When : On appelle la méthode get de mon api pour récupérer le produit
        ActionResult<ProductDetailDTO> action = _productController.Get(produitId).GetAwaiter().GetResult();

        // Then : On ne renvoie rien et on renvoie NOT_FOUND (404)
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");

        _produitManager.Verify(manager => manager.GetByIdAsync(produitId), Times.Once);
    }

    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given : Un produit créé
        Product produitCreated = new()
        {
            IdProduct = 30,
            ProductName = "Chaise",
            Description = "Une superbe chaise",
            PhotoName = "Une superbe chaise bleu",
            PhotoUri = "https://ikea.fr/chaise.jpg"
        };

        ProductAddDTO expectedDto = new()
        {
            Nom = "Chaise",
            Description = "Une superbe chaise",
            NomPhoto = "Une superbe chaise bleu",
            UriPhoto = "https://ikea.fr/chaise.jpg"
        };

        ProductDetailDTO expectedDetailDto = new()
        {
            Id = 30,
            Name = "Chaise",
            Description = "Une superbe chaise"
        };

        _produitManager
            .Setup(manager => manager.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(produitCreated);

        _productAddMapper
            .Setup(mapper => mapper.ToDTO(produitCreated))
            .Returns(expectedDto);

        _produitDetailMapper
            .Setup(mapper => mapper.ToDTO(produitCreated))
            .Returns(expectedDetailDto);

        // When : On appel la méthode POST de l'API pour enregistrer le produit
        ActionResult<ProductDetailDTO> action = _productController.Create(expectedDto).GetAwaiter().GetResult();

        // Then : Le produit est bien enregistré et le code renvoyé et CREATED (201)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));

        _produitManager.Verify(manager => manager.AddAsync(It.IsAny<Product>()), Times.Once);
        _produitDetailMapper.Verify(mapper => mapper.ToDTO(produitCreated), Times.Once);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // Given : Un produit à mettre à jour
        Product produitToEdit = new()
        {
            IdProduct = 20,
            ProductName = "Bureau",
            Description = "Un super bureau",
            PhotoName = "Un super bureau bleu",
            PhotoUri = "https://ikea.fr/bureau.jpg"
        };

        _produitManager
            .Setup(manager => manager.GetByIdAsync(produitToEdit.IdProduct))
            .ReturnsAsync(produitToEdit);

        _produitManager
            .Setup(manager => manager.UpdateAsync(produitToEdit, produitToEdit))
            .Returns(Task.CompletedTask);

        // When : On appelle la méthode PUT du controller pour mettre à jour le produit
        IActionResult action = _productController.Update(produitToEdit.IdProduct, produitToEdit).GetAwaiter().GetResult();

        // Then : On vérifie que le produit a bien été modifié et que le code renvoyé et NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        _produitManager.Verify(manager => manager.GetByIdAsync(produitToEdit.IdProduct), Times.Once);
        _produitManager.Verify(manager => manager.UpdateAsync(produitToEdit, produitToEdit), Times.Once);
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseIdInUrlIsDifferent()
    {
        // Given : Un produit à mettre à jour
        Product produitToEdit = new()
        {
            IdProduct = 20,
            ProductName = "Bureau",
            Description = "Un super bureau",
            PhotoName = "Un super bureau bleu",
            PhotoUri = "https://ikea.fr/bureau.jpg"
        };

        // When : On appelle la méthode PUT du controller pour mettre à jour le produit,
        // mais en précisant un ID différent de celui du produit enregistré
        IActionResult action = _productController.Update(1, produitToEdit).GetAwaiter().GetResult();

        // Then : On vérifie que l'API renvoie un code BAD_REQUEST (400)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));

        _produitManager.Verify(manager => manager.GetByIdAsync(produitToEdit.IdProduct), Times.Never);
        _produitManager.Verify(manager => manager.UpdateAsync(produitToEdit, produitToEdit), Times.Never);
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // Given : Un produit à mettre à jour qui n'est pas enregistré
        Product produitToEdit = new()
        {
            IdProduct = 20,
            ProductName = "Bureau",
            Description = "Un super bureau",
            PhotoName = "Un super bureau bleu",
            PhotoUri = "https://ikea.fr/bureau.jpg"
        };

        _produitManager
            .Setup(manager => manager.GetByIdAsync(produitToEdit.IdProduct))
            .ReturnsAsync((Product?)null);

        // When : On appelle la méthode PUT du controller pour mettre à jour un produit qui n'est pas enregistré
        IActionResult action = _productController.Update(produitToEdit.IdProduct, produitToEdit).GetAwaiter().GetResult();

        // Then : On vérifie que l'API renvoie un code NOT_FOUND (404)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));

        _produitManager.Verify(manager => manager.GetByIdAsync(produitToEdit.IdProduct), Times.Once);
        _produitManager.Verify(manager => manager.UpdateAsync(produitToEdit, produitToEdit), Times.Never);
    }
}