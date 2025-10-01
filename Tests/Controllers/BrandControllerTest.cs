using App.Controllers;
using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(BrandController))]
[TestCategory("integration")]
public class BrandControllerTest
{
    private AppDbContext _context;
    private BrandController _brandController;
    private MapperConfiguration _config;
    private IMapper _mapper;
    private IDataRepository<Brand> _manager;

    // Objets communs pour les tests
    private Brand _brandAdidas;
    private Brand _brandNike;
    private Brand _brandCorsairEntity;
    private BrandDTO _brandDtoCorsair;

    [TestInitialize]
    public void Initialize()
    {
        // Contexte et mapper
        _context = new AppDbContext();

        _config = new MapperConfiguration(cfg => cfg.AddProfile<BrandMapper>(), new LoggerFactory());
        _config.AssertConfigurationIsValid();
        _mapper = _config.CreateMapper();

        // Manager et controller
        _manager = new BrandManager(_context);
        _brandController = new BrandController(_mapper, _manager, _context);

        // Données communes : entities
        _brandAdidas = new Brand { BrandName = "Adidas" };
        _brandNike = new Brand { BrandName = "Nike" };

        // DTO
        _brandDtoCorsair = new BrandDTO { Name = "Corsair" };

        // Mapper DTO → Entity et ajouter en DB
        _brandCorsairEntity = _mapper.Map<Brand>(_brandDtoCorsair);

        // Ajout initial en DB
        _context.Brands.AddRange(_brandAdidas, _brandNike, _brandCorsairEntity);
        _context.SaveChanges();
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<BrandDTO> action = _brandController.Get(_brandAdidas.IdBrand).GetAwaiter().GetResult();
        BrandDTO returnProduct = action.Value; 

        // Then : On récupère le produit et le code de retour est 200
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(BrandDTO));
        Assert.AreEqual(_brandAdidas.BrandName, returnProduct.Name);
    }

    [TestMethod]
    public void ShouldDeleteBrand()
    {
        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _brandController.Delete(_brandAdidas.IdBrand).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Products.Find(_brandAdidas.IdBrand));
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseBrandDoesNotExist()
    {
        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _brandController.Delete(_brandAdidas.IdBrand).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllBrands()
    {
        // When : On souhaite récupérer tous les produits
        var products = _brandController.GetAll().GetAwaiter().GetResult();

        // Then : Tous les produits sont récupérés
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products.Value, typeof(IEnumerable<Brand>));
    }

    [TestMethod]
    public void GetBrandShouldReturnNotFound()
    {
        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<BrandDTO> action = _brandController.Get(0).GetAwaiter().GetResult(); ;

        // Then : On ne renvoie rien et on renvoie 404
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
    }

    [TestMethod]
    public void ShouldCreateBrand()
    {
        // When
        ActionResult<BrandDTO> action = _brandController.Create(_brandDtoCorsair).GetAwaiter().GetResult(); // ✅ Corrigé le type

        // Then
        var createdResult = (CreatedAtActionResult)action.Result;
        var createdDto = (BrandDTO)createdResult.Value;

        Brand brandInDb = _context.Brands.Find(createdDto.Name);  // pas sûr de ça 

        Assert.IsNotNull(brandInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        Assert.AreEqual(_brandDtoCorsair.Name, brandInDb.BrandName);
    }

    [TestMethod]
    public void ShouldUpdateBrand()
    {
        // Given
        _brandAdidas.BrandName = "Puma";

        // When
        IActionResult action = _brandController.Update(_brandAdidas.IdBrand, _brandAdidas).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        Product editedBrandInDb = _context.Products.Find(_brandAdidas.IdBrand);

        Assert.IsNotNull(editedBrandInDb);
        Assert.AreEqual(_brandAdidas.BrandName, editedBrandInDb.ProductName);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseIdInUrlIsDifferent()
    {
        _context.Brands.Add(_brandNike);
        _context.SaveChanges();

        _brandNike.BrandName = "OnlyFan";

        // When
        IActionResult action = _brandController.Update(0, _brandNike).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseBrandDoesNotExist()
    {
        // When
        IActionResult action = _brandController.Update(_brandNike.IdBrand, _brandNike).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Brands.RemoveRange(_context.Brands);
        _context.SaveChanges();
    }
}
